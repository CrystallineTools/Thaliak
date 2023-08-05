variable "TAG" {
    default = "2"
}

group "default" {
    targets = [
        "poller",
        "api"
    ]
}

target "poller" {
    target = "thaliak-poller"
    tags = ["ghcr.io/avafloww/thaliak-poller:${TAG}"]
}

target "api" {
    target = "thaliak-api"
    tags = ["ghcr.io/avafloww/thaliak-api:${TAG}"]
}
