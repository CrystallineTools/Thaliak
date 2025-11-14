use axum::{
    extract::{Request, State},
    http::StatusCode,
    middleware::Next,
    response::{IntoResponse, Response},
};

use crate::db::AppState;

fn parse_cookie_value(cookie_header: &str, name: &str) -> Option<String> {
    cookie_header
        .split(';')
        .filter_map(|cookie| {
            let cookie = cookie.trim();
            if let Some((key, value)) = cookie.split_once('=') {
                if key == name {
                    return Some(value.to_string());
                }
            }
            None
        })
        .next()
}

pub async fn auth_middleware(
    State(state): State<AppState>,
    mut req: Request,
    next: Next,
) -> Result<Response, AuthError> {
    let auth = state.auth.as_ref().expect("auth should be configured");

    let token = req
        .headers()
        .get("cookie")
        .and_then(|h| h.to_str().ok())
        .and_then(|cookies| parse_cookie_value(cookies, "thaliak_token"))
        .ok_or(AuthError::MissingToken)?;

    let claims = auth
        .jwt_manager
        .validate_token(&token)
        .map_err(|_| AuthError::InvalidToken)?;

    req.extensions_mut().insert(claims);

    Ok(next.run(req).await)
}

#[derive(Debug)]
pub enum AuthError {
    MissingToken,
    InvalidToken,
}

impl IntoResponse for AuthError {
    fn into_response(self) -> Response {
        let (status, message) = match self {
            AuthError::MissingToken => (StatusCode::UNAUTHORIZED, "Missing authentication token"),
            AuthError::InvalidToken => (StatusCode::UNAUTHORIZED, "Invalid authentication token"),
        };

        (status, message).into_response()
    }
}
