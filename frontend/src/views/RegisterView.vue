<template>
  <div class="min-h-screen flex items-center justify-center bg-gradient-to-br from-indigo-600 via-purple-600 to-pink-400 px-4">
    <div
      class="backdrop-blur-md bg-white/20 border border-white/40 shadow-2xl rounded-2xl p-10 w-full max-w-md text-white transition-all hover:shadow-indigo-400/50"
    >
      <!-- Header -->
      <div class="text-center mb-8">
        <h1 class="text-3xl font-extrabold mb-2 drop-shadow-lg">Create Account ✨</h1>
        <p class="text-sm text-indigo-100/90">Sign up and start tracking your expenses</p>
      </div>

      <!-- Registration Form -->
      <form @submit.prevent="register" class="space-y-6">
        <div>
          <label class="block text-sm font-semibold mb-1 text-indigo-50">Username</label>
          <input
            v-model="username"
            type="text"
            placeholder="Your username"
            class="w-full px-4 py-3 rounded-lg border-none bg-white/80 text-gray-800 placeholder-gray-500 focus:ring-4 focus:ring-indigo-300 focus:outline-none"
          />
        </div>

        <div>
          <label class="block text-sm font-semibold mb-1 text-indigo-50">Email</label>
          <input
            v-model="email"
            type="email"
            placeholder="you@example.com"
            class="w-full px-4 py-3 rounded-lg border-none bg-white/80 text-gray-800 placeholder-gray-500 focus:ring-4 focus:ring-indigo-300 focus:outline-none"
          />
        </div>

        <div>
          <label class="block text-sm font-semibold mb-1 text-indigo-50">Password</label>
          <input
            v-model="password"
            type="password"
            placeholder="Create a strong password"
            class="w-full px-4 py-3 rounded-lg border-none bg-white/80 text-gray-800 placeholder-gray-500 focus:ring-4 focus:ring-indigo-300 focus:outline-none"
          />
        </div>

        <button
          type="submit"
          class="w-full py-3 bg-indigo-600 hover:bg-indigo-700 text-white font-semibold rounded-lg shadow-md hover:shadow-lg transition-all duration-300"
        >
          Register
        </button>
      </form>

      <!-- Error Message -->
      <p v-if="error" class="text-red-300 text-center text-sm mt-4">{{ error }}</p>

      <!-- Divider -->
      <div class="flex items-center my-6">
        <div class="flex-1 h-px bg-white/40"></div>
        <p class="text-sm text-white/80 mx-3">or</p>
        <div class="flex-1 h-px bg-white/40"></div>
      </div>

      <!-- Redirect -->
      <p class="text-center text-white/90 text-sm">
        Already have an account?
        <RouterLink
          to="/login"
          class="text-yellow-200 font-semibold hover:underline hover:text-yellow-300 transition"
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
