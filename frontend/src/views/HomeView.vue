<template>
  <div
    class="relative min-h-screen bg-gradient-to-b from-[#111] via-[#141517] to-[#0f0f10] text-gray-100 flex flex-col items-center text-center px-6 pt-20 pb-16"
  >
    <!-- 🌌 Background Particles -->
    <div class="particle-bg"></div>

    <!-- ⭐ Continuous Infinite Scrolling Banner -->
    <div class="banner-shell mb-10">
      <div class="banner-track">
        <!-- First lane -->
        <div class="banner-lane">
          <span
            v-for="(w, i) in bannerWords"
            :key="'lane1-' + i"
            class="banner-item"
          >
            {{ w }}
          </span>
        </div>

        <!-- Second lane (duplicate for seamless loop) -->
        <div class="banner-lane">
          <span
            v-for="(w, i) in bannerWords"
            :key="'lane2-' + i"
            class="banner-item"
          >
            {{ w }}
          </span>
        </div>
      </div>
    </div>

    <!-- 💸 Hero Section -->
    <h1 class="text-4xl md:text-5xl font-extrabold mb-3">
      Take Control of Your Finances 💰
    </h1>

    <!-- ✨ Rotating subtitle -->
    <h2 class="text-xl md:text-2xl text-indigo-400 font-semibold h-8 overflow-hidden mb-6">
      <transition-group name="fade-up" tag="div">
        <div :key="currentWord" class="animated-word">
          {{ currentWord }}
        </div>
      </transition-group>
    </h2>

    <p class="text-gray-400 max-w-xl mb-8">
      Track expenses effortlessly, set budgets with ease, and visualize your spending
      habits with clear, modern charts.
    </p>

    <!-- 🚀 CTAs -->
    <div class="flex flex-wrap justify-center gap-4 mb-12">
      <RouterLink to="/register" class="btn-primary">
        Get Started — It’s Free
      </RouterLink>

      <RouterLink to="/login" class="btn-secondary">
        Already have an account? Log In
      </RouterLink>
    </div>

    <!-- 💡 Feature Highlights -->
    <section class="grid md:grid-cols-3 gap-6 max-w-5xl w-full">
      <div class="feature-card">
        <h3 class="feature-title">📊 Visual Insights</h3>
        <p class="feature-text">
          Watch your spending patterns come to life with elegant charts.
        </p>
      </div>

      <div class="feature-card">
        <h3 class="feature-title">🎯 Smart Budgets</h3>
        <p class="feature-text">
          Set realistic goals and get notified before you overspend.
        </p>
      </div>

      <div class="feature-card">
        <h3 class="feature-title">📆 Stay on Track</h3>
        <p class="feature-text">
          Build healthy financial habits with consistent tracking.
        </p>
      </div>
    </section>
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";

// Rotating subtitle under main heading
const rotatingWords = [
  "Track. Budget. Grow.",
  "Understand Your Spending.",
  "Master Your Money.",
  "Stay Financially Confident.",
  "Build Smarter Habits.",
];

const currentWord = ref(rotatingWords[0]);
let wordIndex = 0;

onMounted(() => {
  setInterval(() => {
    wordIndex = (wordIndex + 1) % rotatingWords.length;
    currentWord.value = rotatingWords[wordIndex];
  }, 2500);
});

// Banner words
const bannerWords = [
  "Simple",
  "Smart",
  "Secure",
  "Modern",
  "Fast",
  "Clean",
  "Minimal",
  "Powerful",
];
</script>

<style scoped>
/* 💫 Slim, smooth feature cards */
.feature-card {
  padding: 1.5rem;
  border-radius: 1.2rem;
  background: rgba(29, 31, 34, 0.55);
  border: 1px solid rgba(53, 56, 62, 0.4);
  backdrop-filter: blur(6px);
  transition: 0.2s ease;
}
.feature-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 0 14px rgba(99, 102, 241, 0.25);
}
.feature-title {
  color: #818cf8;
  font-size: 1.05rem;
  font-weight: 600;
  margin-bottom: 0.35rem;
}
.feature-text {
  font-size: 0.9rem;
  color: #d1d5db;
}

/* 🎛 Buttons */
.btn-primary {
  background: #6366f1;
  padding: 0.75rem 1.6rem;
  border-radius: 0.7rem;
  font-weight: 600;
  color: white;
  transition: 0.2s;
}
.btn-primary:hover {
  background: #4f51e8;
  box-shadow: 0 0 20px rgba(99, 102, 241, 0.3);
}
.btn-secondary {
  border: 1px solid #555;
  padding: 0.75rem 1.6rem;
  border-radius: 0.7rem;
  color: #e5e7eb;
  transition: 0.2s;
}
.btn-secondary:hover {
  border-color: #818cf8;
  color: white;
}

/* 🔁 Rotating subtitle animation */
.fade-up-enter-active,
.fade-up-leave-active {
  transition: all 0.5s ease;
}
.fade-up-enter-from,
.fade-up-leave-to {
  opacity: 0;
  transform: translateY(10px);
}
.animated-word {
  height: 32px; /* keeps layout from jumping */
}

/* 🧷 Marquee banner (no gaps) */
.banner-shell {
  width: 100%;
  overflow: hidden;
}
.banner-track {
  display: flex;
  width: 200%;           /* two lanes side-by-side */
  animation: banner-slide 20s linear infinite;
}
.banner-lane {
  flex: 0 0 50%;         /* each lane always fills full viewport width */
  display: flex;
  justify-content: space-around;
  align-items: center;
}
.banner-item {
  font-size: 1.05rem;
  color: #e5e7eb;
  opacity: 0.8;
  white-space: nowrap;
}
@keyframes banner-slide {
  0% {
    transform: translateX(0);
  }
  100% {
    transform: translateX(-50%);
  }
}

/* 🌌 Particle background */
.particle-bg {
  position: absolute;
  inset: 0;
  z-index: -10;
  pointer-events: none;
  background:
    radial-gradient(circle at 20% 30%, rgba(99, 102, 241, 0.18), transparent 60%),
    radial-gradient(circle at 80% 70%, rgba(236, 72, 153, 0.13), transparent 60%),
    radial-gradient(circle at 50% 50%, rgba(59, 130, 246, 0.1), transparent 70%);
  animation: drift 20s infinite alternate ease-in-out;
}
@keyframes drift {
  0% {
    background-position: 20% 30%;
  }
  100% {
    background-position: 70% 40%;
  }
}
</style>
