<template>
  <div class="auth-page">
    <h2>Register</h2>
    <input v-model="username" placeholder="Username" />
    <input v-model="email" placeholder="Email" />
    <input v-model="password" type="password" placeholder="Password" />
    <button @click="register">Register</button>
    <p>{{ error }}</p>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import api from '../api'

const username = ref('')
const email = ref('')
const password = ref('')
const error = ref('')
const router = useRouter()

const register = async () => {
  try {
    const res = await api.post('/auth/register', {
      username: username.value,
      email: email.value,
      password: password.value
    })
    localStorage.setItem('token', res.data.token)
    router.push('/dashboard')
  } catch {
    error.value = 'Registration failed. Email may already exist.'
  }
}
</script>
