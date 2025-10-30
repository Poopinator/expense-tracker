// 🌐 Core Vue imports
import { createApp } from "vue"
import App from "./App.vue"
import router from "./router"

// 🎨 Global Styles (MUST come before everything else)
import "./style.css" // includes Tailwind + base resets

// 🔔 Toast Notifications
import Toast from "vue-toastification"
import "vue-toastification/dist/index.css"

// 🪩 Create Vue app
const app = createApp(App)

// 🧭 Register router
app.use(router)

// 🍞 Configure Toast notifications
app.use(Toast, {
  position: "top-right",
  timeout: 3000,
  closeOnClick: true,
  pauseOnHover: true,
  draggable: true,
  draggablePercent: 0.6,
  hideProgressBar: false,
  showCloseButtonOnHover: true,
  transition: "Vue-Toastification__fade",
})

// 🚀 Mount app
app.mount("#app")
