use axum::{
    extract::FromRequestParts,
    http::{StatusCode, request::Parts},
    response::{IntoResponse, Response},
};
use std::future::ready;

use super::Claims;

pub struct AuthenticatedUser(pub Claims);

impl<S> FromRequestParts<S> for AuthenticatedUser
where
    S: Send + Sync,
{
    type Rejection = AuthExtractorError;

    fn from_request_parts(
        parts: &mut Parts,
        _state: &S,
    ) -> impl std::future::Future<Output = Result<Self, Self::Rejection>> + Send {
        ready(
            parts
                .extensions
                .get::<Claims>()
                .cloned()
                .map(AuthenticatedUser)
                .ok_or(AuthExtractorError),
        )
    }
}

pub struct AuthExtractorError;

impl IntoResponse for AuthExtractorError {
    fn into_response(self) -> Response {
        (
            StatusCode::UNAUTHORIZED,
            "Missing or invalid authentication",
        )
            .into_response()
    }
}
