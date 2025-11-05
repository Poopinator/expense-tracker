<script setup>
import { ref, watchEffect, onMounted } from "vue"
import { RouterView, useRoute, useRouter } from "vue-router"
import Navbar from "../src/components/Navbar.vue"

const route = useRoute()
const router = useRouter()

// ✅ reactive auth state
const loggedIn = ref(false)

// ✅ check token validity (JWT-aware)
const checkAuth = () => {
  const token = localStorage.getItem("token")
  if (!token) return false

  try {
    // Decode JWT to check expiry
    const payload = JSON.parse(atob(token.split(".")[1]))
    const isExpired = payload.exp * 1000 < Date.now()
    if (isExpired) {
      localStorage.removeItem("token")
      return false
    }
    return true
  } catch {
    localStorage.removeItem("token")
    return false
  }
}

// ✅ update auth state on route change
watchEffect(() => {
  loggedIn.value = checkAuth()
})

// ✅ run check on load
onMounted(() => {
  loggedIn.value = checkAuth()
})

// 🚪 logout function
const logout = () => {
  localStorage.removeItem("token")
  loggedIn.value = false
  router.push("/login")
}
</script>

<template>
  <div id="app" class="min-h-screen flex flex-col bg-[#0d0d0d] text-gray-100 font-sans">
    <!-- ✅ Navbar only for authenticated routes -->
    <Navbar
      v-if="loggedIn && !['/login', '/register', '/'].includes(route.path)"
      @logout="logout"
    />

    <!-- 🧭 Main Page Content -->
    <main class="flex-1">
      <RouterView />
    </main>

    <!-- ✅ Footer only for authenticated users -->
    <footer
      v-if="loggedIn && !['/login', '/register', '/'].includes(route.path)"
      class="text-center text-sm text-gray-500 py-6 border-t border-gray-800"
    >
      © {{ new Date().getFullYear() }} ExpenseMate — Track smart, spend smarter 💸
    </footer>
  </div>
</template>

<style>
html,
body {
  margin: 0;
  padding: 0;
  font-family: 'Inter', sans-serif;
  background-color: #0d0d0d;
}
</style>
