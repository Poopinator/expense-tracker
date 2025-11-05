<template>
  <div class="relative min-h-screen bg-gradient-to-b from-[#111] via-[#141517] to-[#0f0f10] text-gray-100 flex flex-col items-center justify-center text-center px-6">
    <!-- 🌟 Background particles -->
    <div class="particle-bg"></div>

    <!-- 💸 Hero Section -->
    <h1 class="text-4xl md:text-5xl font-extrabold mb-4">
      Take Control of Your Finances 💰
    </h1>
    <p class="text-gray-400 max-w-xl mb-8">
      Track expenses effortlessly, set budgets with ease, and visualize your spending habits with clear, modern charts.
    </p>

    <!-- 🚀 CTA Buttons -->
    <div class="flex flex-wrap justify-center gap-4">
      <RouterLink
        to="/register"
        class="bg-indigo-600 hover:bg-indigo-500 text-white font-semibold px-6 py-3 rounded-lg shadow-lg hover:shadow-indigo-500/30 transition-all duration-200"
      >
        Get Started — It’s Free
      </RouterLink>
      <RouterLink
        to="/login"
        class="border border-gray-600 text-gray-300 hover:text-white hover:border-indigo-400 px-6 py-3 rounded-lg transition-all duration-200"
      >
        Already have an account? Log In
      </RouterLink>
    </div>

    <!-- 💡 Feature Highlights -->
    <section class="mt-20 grid md:grid-cols-3 gap-8 max-w-6xl w-full">
      <div class="p-6 rounded-2xl bg-[#1d1f22]/70 border border-[#35383E] shadow-lg hover:shadow-indigo-500/20 transition">
        <h3 class="text-xl font-semibold text-indigo-400 mb-2">📊 Visual Insights</h3>
        <p class="text-gray-400 text-sm">See where your money goes each month with clean, interactive charts.</p>
      </div>

      <div class="p-6 rounded-2xl bg-[#1d1f22]/70 border border-[#35383E] shadow-lg hover:shadow-indigo-500/20 transition">
        <h3 class="text-xl font-semibold text-indigo-400 mb-2">🎯 Smart Budgets</h3>
        <p class="text-gray-400 text-sm">Set spending limits and get notified when you’re nearing them.</p>
      </div>

      <div class="p-6 rounded-2xl bg-[#1d1f22]/70 border border-[#35383E] shadow-lg hover:shadow-indigo-500/20 transition">
        <h3 class="text-xl font-semibold text-indigo-400 mb-2">📆 Stay on Track</h3>
        <p class="text-gray-400 text-sm">Plan, monitor, and achieve your financial goals — one step at a time.</p>
      </div>
    </section>

    <!-- 📎 Footer CTA -->
    <p class="mt-16 text-gray-500 text-sm">
      Made with ❤️ by ExpenseMate. Your personal finance companion.
    </p>
  </div>
</template>

<script setup>
// no special logic needed — purely presentational
</script>

<style scoped>
.particle-bg {
  position: absolute;
  inset: 0;
  z-index: -10;
  pointer-events: none;
  background: radial-gradient(circle at 20% 30%, rgba(99,102,241,0.15), transparent 60%),
              radial-gradient(circle at 80% 70%, rgba(236,72,153,0.1), transparent 60%),
              radial-gradient(circle at 50% 50%, rgba(59,130,246,0.08), transparent 70%);
  animation: drift 20s infinite alternate ease-in-out;
}

@keyframes drift {
  0% { background-position: 20% 30%, 80% 70%, 50% 50%; }
  100% { background-position: 70% 40%, 30% 60%, 60% 50%; }
}
</style>
