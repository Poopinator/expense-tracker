<template>
  <nav class="sticky top-0 z-40 w-full border-b border-[#2b2e34] bg-[#121316]/90 backdrop-blur">
    <div class="mx-auto max-w-7xl px-4 py-3 flex items-center justify-between">
      <!-- brand -->
      <RouterLink to="/dashboard" class="flex items-center gap-2">
        <span class="text-xl font-bold text-white">💸 ExpenseMate</span>
      </RouterLink>

      <!-- links -->
      <div class="hidden md:flex items-center gap-6">
        <RouterLink
          v-for="item in items"
          :key="item.to"
          :to="item.to"
          class="text-sm font-medium transition"
          :class="isActive(item.to) ? 'text-white' : 'text-gray-400 hover:text-white'"
        >
          {{ item.label }}
        </RouterLink>
      </div>

      <!-- right side: theme + user -->
      <div class="flex items-center gap-3">
        <!-- theme toggle -->
        <button
          class="px-3 py-1.5 rounded-md bg-[#222428] text-gray-200 hover:text-white transition"
          @click="toggleTheme"
          :title="theme === 'dark' ? 'Switch to Light' : 'Switch to Dark'"
        >
          <span v-if="theme==='dark'">🌞</span>
          <span v-else>🌙</span>
        </button>

        <!-- logout -->
        <button
          class="px-3 py-1.5 rounded-md bg-indigo-600 hover:bg-indigo-500 text-white font-semibold transition"
          @click="logout"
        >
          Logout
        </button>

        <!-- mobile menu (optional minimal) -->
        <div class="md:hidden">
          <details class="relative">
            <summary class="px-3 py-1.5 rounded-md bg-[#222428] text-gray-200 cursor-pointer">☰</summary>
            <div class="absolute right-0 mt-2 w-40 rounded-md border border-[#2b2e34] bg-[#121316] p-2">
              <RouterLink
                v-for="item in items"
                :key="item.to"
                :to="item.to"
                class="block px-3 py-2 rounded text-sm text-gray-300 hover:bg-[#1b1d21]"
              >{{ item.label }}</RouterLink>
            </div>
          </details>
        </div>
      </div>
    </div>
  </nav>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute, useRouter, RouterLink } from 'vue-router'

const route = useRoute()
const router = useRouter()

const items = [
  { label: 'Dashboard', to: '/dashboard' },
  { label: 'Reports',   to: '/reports'   },
  { label: 'Settings',  to: '/settings'  }, // we’ll stub this later
]

const isActive = (to) => route.path.startsWith(to)

const theme = ref('dark')
onMounted(() => {
  const saved = localStorage.getItem('theme') || 'dark'
  theme.value = saved
  applyTheme()
})

function applyTheme() {
  const root = document.documentElement
  if (theme.value === 'dark') root.classList.add('dark')
  else root.classList.remove('dark')
  localStorage.setItem('theme', theme.value)
}
function toggleTheme() {
  theme.value = theme.value === 'dark' ? 'light' : 'dark'
  applyTheme()
}

function logout() {
  localStorage.removeItem('token')
  router.push('/')
}
</script>
