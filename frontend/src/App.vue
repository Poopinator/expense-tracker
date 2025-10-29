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
  <div id="app">
    <header v-if="!['/login', '/register'].includes(route.path)">
      <h1>💸 Expense Tracker</h1>
      <button @click="logout">Logout</button>
    </header>

    <main>
      <RouterView />
    </main>
  </div>
</template>

<style scoped>
header {
  background: #2c3e50;
  color: white;
  padding: 1rem 2rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
}
button {
  background: #e74c3c;
  color: white;
  border: none;
  padding: 0.5rem 1rem;
  border-radius: 4px;
  cursor: pointer;
}
button:hover {
  background: #c0392b;
}
main {
  padding: 2rem;
}
</style>
