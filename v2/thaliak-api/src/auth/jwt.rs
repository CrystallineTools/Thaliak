use chrono::{Duration, Utc};
use eyre::Result;
use jsonwebtoken::{DecodingKey, EncodingKey, Header, Validation, decode, encode};
use serde::{Deserialize, Serialize};

const JWT_EXPIRATION_DAYS: i64 = 30;

#[derive(Debug, Clone, Serialize, Deserialize)]
pub struct Claims {
    pub sub: String,
    pub discord_user_id: String,
    pub user_id: i64,
    pub exp: i64,
    pub iat: i64,
}

pub struct JwtManager {
    encoding_key: EncodingKey,
    decoding_key: DecodingKey,
}

impl JwtManager {
    pub fn new(secret: &str) -> Self {
        Self {
            encoding_key: EncodingKey::from_secret(secret.as_bytes()),
            decoding_key: DecodingKey::from_secret(secret.as_bytes()),
        }
    }

    pub fn generate_token(&self, user_id: i64, discord_user_id: &str) -> Result<String> {
        let now = Utc::now();
        let expiration = now + Duration::days(JWT_EXPIRATION_DAYS);

        let claims = Claims {
            sub: discord_user_id.to_string(),
            discord_user_id: discord_user_id.to_string(),
            user_id,
            exp: expiration.timestamp(),
            iat: now.timestamp(),
        };

        let token = encode(&Header::default(), &claims, &self.encoding_key)?;
        Ok(token)
    }

    pub fn validate_token(&self, token: &str) -> Result<Claims> {
        let validation = Validation::default();
        let token_data = decode::<Claims>(token, &self.decoding_key, &validation)?;
        Ok(token_data.claims)
    }
}
