/// <reference types="vite/client" />

interface ImportMetaEnv {
  readonly VITE_GRAPHQL_URL?: string;
  readonly VITE_GIT_SHA?: string;
}

interface ImportMeta {
  readonly env: ImportMetaEnv;
}
