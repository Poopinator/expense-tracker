<template>
  <div>
    <h2>Welcome to your Expense Tracker</h2>

    <section>
      <h3>Add Expense</h3>
      <input v-model="title" placeholder="Title" />
      <input v-model="category" placeholder="Category" />
      <input v-model.number="amount" placeholder="Amount" type="number" />
      <input v-model="date" type="date" />
      <button @click="addExpense">Add</button>
    </section>

    <section>
      <h3>Your Expenses</h3>
      <select v-model="filter">
        <option value="daily">Daily</option>
        <option value="monthly">Monthly</option>
        <option value="all">All</option>
      </select>

      <ul>
        <li v-for="e in filteredExpenses" :key="e.id">
          {{ e.title }} — ${{ e.amount }} ({{ e.category }})
          <button @click="deleteExpense(e.id)">🗑️</button>
        </li>
      </ul>
    </section>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import api from '../api'

const expenses = ref([])
const title = ref('')
const category = ref('')
const amount = ref('')
const date = ref('')
const filter = ref('daily')

const fetchExpenses = async () => {
  const res = await api.get('/api/expense')
  expenses.value = res.data
}

const addExpense = async () => {
  await api.post('/api/expense', {
    title: title.value,
    category: category.value,
    amount: amount.value,
    date: date.value
  })
  await fetchExpenses()
}

const deleteExpense = async (id) => {
  await api.delete(`/api/expense/${id}`)
  await fetchExpenses()
}

const filteredExpenses = computed(() => {
  const now = new Date()
  if (filter.value === 'daily') {
    return expenses.value.filter(e => new Date(e.date).toDateString() === now.toDateString())
  }
  if (filter.value === 'monthly') {
    return expenses.value.filter(e => {
      const d = new Date(e.date)
      return d.getMonth() === now.getMonth() && d.getFullYear() === now.getFullYear()
    })
  }
  return expenses.value
})

onMounted(fetchExpenses)
</script>
