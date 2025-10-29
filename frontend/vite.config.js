import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

// ✅ No Tailwind imports needed; PostCSS handles it automatically
export default defineConfig({
  plugins: [vue()],
  server: {
    port: 5173,
    open: true,
  },
})
