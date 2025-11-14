use eyre::Result;
use log::warn;
use serde::{Deserialize, Serialize};

const DISCORD_API_ENDPOINT: &str = "https://discord.com/api/v10";

#[derive(Debug, Clone)]
pub struct DiscordOAuthClient {
    client_id: String,
    client_secret: String,
    redirect_uri: String,
    bot_token: Option<String>,
    guild_id: Option<String>,
    http_client: reqwest::Client,
}

#[derive(Debug, Serialize)]
struct TokenExchangeRequest {
    client_id: String,
    client_secret: String,
    grant_type: String,
    code: String,
    redirect_uri: String,
}

#[derive(Debug, Deserialize)]
pub struct TokenResponse {
    pub access_token: String,
    #[allow(dead_code)]
    pub token_type: String,
    #[allow(dead_code)]
    pub expires_in: u64,
    #[allow(dead_code)]
    pub refresh_token: String,
    #[allow(dead_code)]
    pub scope: String,
}

#[derive(Debug, Deserialize, Serialize)]
pub struct DiscordUser {
    pub id: String,
    pub username: String,
    pub discriminator: String,
    pub avatar: Option<String>,
    pub global_name: Option<String>,
}

impl DiscordOAuthClient {
    pub fn new(
        client_id: String,
        client_secret: String,
        redirect_uri: String,
        bot_token: Option<String>,
        guild_id: Option<String>,
    ) -> Self {
        Self {
            client_id,
            client_secret,
            redirect_uri,
            bot_token,
            guild_id,
            http_client: reqwest::Client::new(),
        }
    }

    pub fn get_authorization_url(&self, state: &str) -> String {
        let scopes = if self.bot_token.is_some() && self.guild_id.is_some() {
            "identify guilds.join"
        } else {
            "identify"
        };

        format!(
            "https://discord.com/oauth2/authorize?client_id={}&redirect_uri={}&response_type=code&scope={}&state={}",
            self.client_id,
            urlencoding::encode(&self.redirect_uri),
            urlencoding::encode(scopes),
            urlencoding::encode(state)
        )
    }

    pub async fn exchange_code(&self, code: &str) -> Result<TokenResponse> {
        let params = TokenExchangeRequest {
            client_id: self.client_id.clone(),
            client_secret: self.client_secret.clone(),
            grant_type: "authorization_code".to_string(),
            code: code.to_string(),
            redirect_uri: self.redirect_uri.clone(),
        };

        let response = self
            .http_client
            .post(&format!("{}/oauth2/token", DISCORD_API_ENDPOINT))
            .form(&params)
            .send()
            .await?;

        if !response.status().is_success() {
            let status = response.status();
            let text = response.text().await?;
            return Err(eyre::Report::msg(format!(
                "Discord token exchange failed: {} - {}",
                status, text
            )));
        }

        let token_response = response.json::<TokenResponse>().await?;
        Ok(token_response)
    }

    pub async fn get_user_info(&self, access_token: &str) -> Result<DiscordUser> {
        let response = self
            .http_client
            .get(&format!("{}/users/@me", DISCORD_API_ENDPOINT))
            .header("Authorization", format!("Bearer {}", access_token))
            .send()
            .await?;

        if !response.status().is_success() {
            let status = response.status();
            let text = response.text().await?;
            return Err(eyre::Report::msg(format!(
                "Discord user info fetch failed: {} - {}",
                status, text
            )));
        }

        let user = response.json::<DiscordUser>().await?;
        Ok(user)
    }

    pub async fn add_user_to_guild(&self, user_id: &str, access_token: &str) -> Result<()> {
        let (bot_token, guild_id) = match (&self.bot_token, &self.guild_id) {
            (Some(bot), Some(guild)) => (bot, guild),
            _ => {
                warn!("Discord bot token or guild ID not configured, skipping auto-join");
                return Ok(());
            }
        };

        #[derive(Serialize)]
        struct AddGuildMemberBody {
            access_token: String,
        }

        let response = self
            .http_client
            .put(&format!(
                "{}/guilds/{}/members/{}",
                DISCORD_API_ENDPOINT, guild_id, user_id
            ))
            .header("Authorization", format!("Bot {}", bot_token))
            .json(&AddGuildMemberBody {
                access_token: access_token.to_string(),
            })
            .send()
            .await?;

        if response.status().is_success() || response.status().as_u16() == 204 {
            Ok(())
        } else if response.status().as_u16() == 409 {
            Ok(())
        } else {
            let status = response.status();
            let text = response.text().await?;
            warn!("Failed to add user to Discord guild: {} - {}", status, text);
            Ok(())
        }
    }
}
