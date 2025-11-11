import {defineConfig} from 'vite';
import react from '@vitejs/plugin-react';
import {execSync} from 'child_process';
import monacoEditorPlugin from 'vite-plugin-monaco-editor';

// Get git SHA for build
const gitSha = execSync('git rev-parse --short HEAD').toString().trim();

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [
    react(),
    monacoEditorPlugin({
      languageWorkers: ['editorWorkerService', 'json'],
      customWorkers: [
        {
          label: 'graphql',
          entry: 'monaco-graphql/esm/graphql.worker.js'
        }
      ]
    })
  ],
  define: {
    'import.meta.env.VITE_GIT_SHA': JSON.stringify(gitSha),
  },
  server: {
    port: 3000,
    open: true,
  },
  build: {
    outDir: 'build',
    sourcemap: true,
  },
});
