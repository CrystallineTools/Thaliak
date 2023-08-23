use axum::{
    extract::{ws::WebSocket, Path, WebSocketUpgrade},
    response::Response,
    routing::get,
    Router,
};

pub fn router() -> Router {
    Router::new().route("/connect/:id", get(agent_handler))
}

pub async fn agent_handler(Path(client_id): Path<String>, ws: WebSocketUpgrade) -> Response {
    ws.on_upgrade(handle_agent_socket)
}

async fn handle_agent_socket(mut socket: WebSocket) {
    while let Some(msg) = socket.recv().await {
        let msg = if let Ok(msg) = msg {
            msg
        } else {
            // client disconnected
            return;
        };

        if socket.send(msg).await.is_err() {
            // client disconnected
            return;
        }
    }
}
