<script setup>
import { RouterView, useRoute, useRouter } from 'vue-router'

const route = useRoute()
const router = useRouter()

const logout = () => {
  localStorage.removeItem('token')
  router.push('/login')
}
</script>

<template>
  <div id="app" class="min-h-screen flex flex-col bg-gray-50 text-gray-800">
    <!-- Navbar -->
    <header
      v-if="!['/login', '/register'].includes(route.path)"
      class="bg-indigo-600 text-white px-6 py-4 flex justify-between items-center shadow-md"
    >
      <h1 class="text-xl font-semibold tracking-tight">💸 Expense Tracker</h1>
      <button
        @click="logout"
        class="bg-white text-indigo-600 font-semibold px-4 py-2 rounded-lg shadow hover:bg-indigo-100 transition-all duration-200"
      >
        Logout
      </button>
    </header>

    <!-- Page Content -->
    <main class="flex-1">
      <RouterView />
    </main>

    <!-- Footer -->
    <footer
      v-if="!['/login', '/register'].includes(route.path)"
      class="text-center text-sm text-gray-500 py-4 border-t border-gray-200"
    >
      © {{ new Date().getFullYear() }} Expense Tracker — Built with ❤️
    </footer>
  </div>
</template>

<style>
html,
body {
  margin: 0;
  padding: 0;
  font-family: 'Inter', sans-serif;
}
</style>
