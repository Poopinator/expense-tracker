<template>
  <div class="relative min-h-screen bg-[#0d0d0d] text-gray-100 font-sans overflow-hidden">

    <!-- Particles -->
    <div class="particle-bg"></div>

    <!-- Gradient -->
    <div class="absolute inset-0 -z-20 bg-gradient-to-b from-[#141517] via-[#0e0f10] to-black opacity-[0.85]"></div>

    <!-- Wrapper -->
    <div class="relative z-10 flex items-center justify-center min-h-screen px-6">

      <div
        class="login-card animate-fade-in"
      >

        <!-- Logo / Title -->
        <div class="text-center mb-8">
          <div class="mx-auto w-16 h-16 rounded-xl bg-indigo-600/20 border border-indigo-500/30
                      flex items-center justify-center shadow-lg mb-4">
            <span class="text-3xl">💰</span>
          </div>

          <h1 class="text-3xl font-extrabold mb-1">Welcome Back</h1>
          <p class="text-sm text-gray-400">Sign in to continue</p>
        </div>

        <!-- Form -->
        <form @submit.prevent="login" class="space-y-6">

          <!-- Email -->
          <div>
            <label class="input-label">Email</label>
            <input
              v-model="email"
              type="email"
              placeholder="you@example.com"
              class="input-field"
            />
          </div>

          <!-- Password -->
          <div>
            <label class="input-label">Password</label>

            <div class="relative">
              <input
                v-model="password"
                :type="showPassword ? 'text' : 'password'"
                placeholder="Enter your password"
                class="input-field pr-12"
              />

              <!-- Toggle password -->
              <button
                type="button"
                @click="showPassword = !showPassword"
                class="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-indigo-400 transition"
              >
                <span v-if="showPassword">🙈</span>
                <span v-else>👁️</span>
              </button>
            </div>
          </div>

          <!-- Submit -->
          <button
            type="submit"
            class="login-btn"
          >
            Log In
          </button>
        </form>

        <!-- Error -->
        <p v-if="error" class="text-red-400 text-center text-sm mt-4">{{ error }}</p>

        <!-- Divider -->
        <div class="divider my-6">
          <span>or</span>
        </div>

        <p class="text-center text-gray-400 text-sm">
          Need an account?
          <RouterLink to="/register" class="link">
            Create one
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
const showPassword = ref(false)


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
/* Fade In */
@keyframes fadeIn {
  from { opacity: 0; transform: translateY(25px) scale(0.97); }
  to   { opacity: 1; transform: translateY(0) scale(1); }
}

.animate-fade-in {
  animation: fadeIn 1s ease forwards;
}

/* Card Base */
.login-card {
  @apply backdrop-blur-xl bg-[#1d1f22]/70 border border-[#35383E]
         rounded-2xl p-10 w-full max-w-md shadow-2xl relative transition;
}

/* Glowing border gradient animation */
.login-card::before {
  content: "";
  position: absolute;
  inset: -2px;
  border-radius: inherit;
  background: linear-gradient(135deg, #6366f1, #8b5cf6, #3b82f6);
  z-index: -1;
  opacity: 0.15;
  filter: blur(12px);
}

/* Inputs */
.input-label {
  @apply block text-sm font-semibold mb-1 text-gray-300;
}

.input-field {
  @apply w-full px-4 py-3 rounded-lg bg-[#26292E] text-gray-200 placeholder-gray-500
         border border-[#35383E] focus:ring-2 focus:ring-indigo-500 focus:outline-none
         transition-all duration-200;
}

/* CTA Button */
.login-btn {
  @apply w-full py-3 bg-indigo-600 hover:bg-indigo-500 text-white font-semibold rounded-lg
         shadow-md hover:shadow-indigo-500/40 transition-all duration-300
         active:scale-[0.97];
}

/* Divider */
.divider {
  @apply flex items-center justify-center text-gray-500 text-sm relative;
}

.divider::before,
.divider::after {
  content: "";
  height: 1px;
  width: 40%;
  background: #35383E;
  position: absolute;
}

.divider::before { left: 0; }
.divider::after { right: 0; }

.divider span {
  @apply px-3 bg-transparent relative text-gray-400;
}

/* Links */
.link {
  @apply text-indigo-400 font-semibold hover:text-indigo-300 hover:underline transition;
}
</style>

