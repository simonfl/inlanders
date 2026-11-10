import { defineConfig } from 'vite';
export default defineConfig({
  // Dropbox may briefly hold old assets. Content-hashed files need not be deleted for an edit/build.
  build: { emptyOutDir: false, reportCompressedSize: false },
  server: { proxy: { '/api': 'http://127.0.0.1:5180' } }
});
