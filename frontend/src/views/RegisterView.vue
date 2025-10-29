<template>
  <div
    class="relative min-h-screen flex items-center justify-center overflow-hidden bg-[#0f0f10] text-gray-100 font-sans"
  >
    <!-- ✨ Animated Gradient Background -->
    <div class="absolute inset-0 animate-gradient bg-[length:200%_200%]
      bg-gradient-to-br from-[#1a1b1f] via-[#202126] to-[#0f0f10] opacity-90 -z-10">
    </div>

    <!-- 🌌 Subtle Moving Glow Particles (SVG) -->
    <svg
      class="absolute inset-0 w-full h-full opacity-30 -z-10"
      xmlns="http://www.w3.org/2000/svg"
      preserveAspectRatio="none"
    >
      <defs>
        <radialGradient id="g" fx="50%" fy="50%" r="50%">
          <stop offset="0%" stop-color="#6366F1" stop-opacity="0.5" />
          <stop offset="100%" stop-color="transparent" />
        </radialGradient>
      </defs>
      <circle cx="20%" cy="30%" r="250" fill="url(#g)">
        <animate attributeName="cx" values="20%;70%;20%" dur="18s" repeatCount="indefinite" />
      </circle>
      <circle cx="70%" cy="70%" r="250" fill="url(#g)">
        <animate attributeName="cy" values="70%;40%;70%" dur="15s" repeatCount="indefinite" />
      </circle>
    </svg>

    <!-- 💫 Register Card -->
    <div
      class="backdrop-blur-md bg-[#1d1f22]/70 border border-[#35383E] shadow-2xl rounded-2xl p-10 w-full max-w-md text-gray-100 transition-all duration-300 hover:shadow-indigo-500/20 animate-fade-in"
    >
      <!-- Header -->
      <div class="text-center mb-8">
        <h1 class="text-3xl font-extrabold mb-2 text-white tracking-tight">Create Account ✨</h1>
        <p class="text-sm text-gray-400">Sign up and start tracking your expenses</p>
      </div>

      <!-- Registration Form -->
      <form @submit.prevent="register" class="space-y-6">
        <div>
          <label class="block text-sm font-semibold mb-1 text-gray-300">Username</label>
          <input
            v-model="username"
            type="text"
            placeholder="Your username"
            class="w-full px-4 py-3 rounded-lg bg-[#26292E] text-gray-200 placeholder-gray-500 border border-[#35383E] focus:ring-2 focus:ring-indigo-500 focus:outline-none transition-all duration-200"
          />
        </div>

        <div>
          <label class="block text-sm font-semibold mb-1 text-gray-300">Email</label>
          <input
            v-model="email"
            type="email"
            placeholder="you@example.com"
            class="w-full px-4 py-3 rounded-lg bg-[#26292E] text-gray-200 placeholder-gray-500 border border-[#35383E] focus:ring-2 focus:ring-indigo-500 focus:outline-none transition-all duration-200"
          />
        </div>

        <div>
          <label class="block text-sm font-semibold mb-1 text-gray-300">Password</label>
          <input
            v-model="password"
            type="password"
            placeholder="Create a strong password"
            class="w-full px-4 py-3 rounded-lg bg-[#26292E] text-gray-200 placeholder-gray-500 border border-[#35383E] focus:ring-2 focus:ring-indigo-500 focus:outline-none transition-all duration-200"
          />
        </div>

        <button
          type="submit"
          class="w-full py-3 bg-indigo-600 hover:bg-indigo-500 text-white font-semibold rounded-lg shadow-md hover:shadow-indigo-500/30 transition-all duration-300"
        >
          Register
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

      <!-- Redirect -->
      <p class="text-center text-gray-400 text-sm">
        Already have an account?
        <RouterLink
          to="/login"
          class="text-indigo-400 font-semibold hover:underline hover:text-indigo-300 transition"
        >
          Log in here
        </RouterLink>
      </p>
    </div>
  </div>
</template>

<script setup>
import { ref } from "vue"
import { useRouter } from "vue-router"
import api from "../api"

const username = ref("")
const email = ref("")
const password = ref("")
const error = ref("")
const router = useRouter()

const register = async () => {
  try {
    const res = await api.post("/auth/register", {
      username: username.value,
      email: email.value,
      password: password.value,
    })
    localStorage.setItem("token", res.data.token)
    router.push("/dashboard")
  } catch {
    error.value = "Registration failed. Email may already exist."
  }
}
</script>

<style scoped>
@keyframes gradientShift {
  0% {
    background-position: 0% 50%;
  }
  50% {
    background-position: 100% 50%;
  }
  100% {
    background-position: 0% 50%;
  }
}

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

.animate-gradient {
  animation: gradientShift 15s ease infinite;
}

.animate-fade-in {
  animation: fadeIn 1.2s ease forwards;
}
</style>
