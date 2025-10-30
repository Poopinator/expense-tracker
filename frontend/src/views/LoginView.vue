<template>
  <div
    class="relative min-h-screen bg-[#0d0d0d] text-gray-100 font-sans overflow-hidden"
  >
    <!-- 🌌 Tailwind Particle Background (same as Dashboard) -->
    <div class="particle-bg"></div>

    <!-- 🌈 Gradient Overlay (behind particles) -->
    <div class="absolute inset-0 -z-20 bg-gradient-to-b from-[#1c1d20] via-[#141517] to-[#0f1011] opacity-90"></div>

    <!-- 💫 Login Card -->
    <div
      class="relative z-10 flex items-center justify-center min-h-screen px-6"
    >
      <div
        class="backdrop-blur-md bg-[#1d1f22]/70 border border-[#35383E]
        shadow-2xl rounded-2xl p-10 w-full max-w-md text-gray-100 transition-all duration-300
        hover:shadow-indigo-500/20 animate-fade-in"
      >
        <!-- Header -->
        <div class="text-center mb-8">
          <h1 class="text-3xl font-extrabold mb-2 text-white tracking-tight">Welcome Back 👋</h1>
          <p class="text-sm text-gray-400">Log in to manage your expenses efficiently</p>
        </div>

        <!-- Login Form -->
        <form @submit.prevent="login" class="space-y-6">
          <div>
            <label class="block text-sm font-semibold mb-1 text-gray-300">Email</label>
            <input
              v-model="email"
              type="email"
              placeholder="you@example.com"
              class="w-full px-4 py-3 rounded-lg bg-[#26292E] text-gray-200 placeholder-gray-500
              border border-[#35383E] focus:ring-2 focus:ring-indigo-500 focus:outline-none transition-all duration-200"
            />
          </div>

          <div>
            <label class="block text-sm font-semibold mb-1 text-gray-300">Password</label>
            <input
              v-model="password"
              type="password"
              placeholder="Enter your password"
              class="w-full px-4 py-3 rounded-lg bg-[#26292E] text-gray-200 placeholder-gray-500
              border border-[#35383E] focus:ring-2 focus:ring-indigo-500 focus:outline-none transition-all duration-200"
            />
          </div>

          <button
            type="submit"
            class="w-full py-3 bg-indigo-600 hover:bg-indigo-500 text-white font-semibold
            rounded-lg shadow-md hover:shadow-indigo-500/30 transition-all duration-300"
          >
            Log In
          </button>
        </form>

        <!-- Error Message -->
        <p v-if="error" class="text-red-400 text-center text-sm mt-4">{{ error }}</p>

        <!-- Divider -->
        <div class="flex items-center my-6">
          <div class="flex-1 h-px bg-gray-700"></div>
          <p class="text-sm text-gray-400 mx-3">or</p>
          <div class="flex-1 h-px bg-gray-700"></div>
        </div>

        <!-- Register Redirect -->
        <p class="text-center text-gray-400 text-sm">
          Don’t have an account?
          <RouterLink
            to="/register"
            class="text-indigo-400 font-semibold hover:underline hover:text-indigo-300 transition"
          >
            Create one here
          </RouterLink>
        </p>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from "vue"
import { useRouter } from "vue-router"
import api from "../api"

const email = ref("")
const password = ref("")
const error = ref("")
const router = useRouter()

const login = async () => {
  error.value = ""
  try {
    const res = await api.post("/auth/login", {
      email: email.value,
      password: password.value,
    })
    localStorage.setItem("token", res.data.token)
    router.push("/dashboard")
  } catch {
    error.value = "Invalid email or password"
  }
}
</script>

<style scoped>
@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.animate-fade-in {
  animation: fadeIn 1.2s ease forwards;
}
</style>
