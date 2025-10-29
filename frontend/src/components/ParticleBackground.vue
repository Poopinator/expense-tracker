<template>
  <canvas ref="canvas" class="absolute inset-0 -z-10"></canvas>
</template>

<script setup>
import { ref, onMounted, onBeforeUnmount } from "vue"

const canvas = ref(null)
let ctx
let particles = []
let animationFrame

const particleCount = 70
const colors = ["#6366F1", "#60A5FA", "#A5B4FC"]

// 🧩 Create particles
function initParticles() {
  const { width, height } = canvas.value
  particles = Array.from({ length: particleCount }, () => ({
    x: Math.random() * width,
    y: Math.random() * height,
    r: Math.random() * 2 + 1,
    dx: (Math.random() - 0.5) * 0.5,
    dy: (Math.random() - 0.5) * 0.5,
    color: colors[Math.floor(Math.random() * colors.length)],
  }))
}

// 🎨 Animate particles
function drawParticles() {
  const { width, height } = canvas.value
  ctx.clearRect(0, 0, width, height)

  // Draw each particle
  for (const p of particles) {
    ctx.beginPath()
    ctx.arc(p.x, p.y, p.r, 0, Math.PI * 2)
    ctx.fillStyle = p.color
    ctx.globalAlpha = 0.5
    ctx.fill()
    ctx.closePath()

    // Optional connecting lines
    for (const q of particles) {
      const dist = Math.hypot(p.x - q.x, p.y - q.y)
      if (dist < 100) {
        ctx.beginPath()
        ctx.moveTo(p.x, p.y)
        ctx.lineTo(q.x, q.y)
        ctx.strokeStyle = "rgba(99,102,241,0.05)"
        ctx.lineWidth = 1
        ctx.stroke()
        ctx.closePath()
      }
    }

    // Move
    p.x += p.dx
    p.y += p.dy
    if (p.x < 0 || p.x > width) p.dx *= -1
    if (p.y < 0 || p.y > height) p.dy *= -1
  }

  animationFrame = requestAnimationFrame(drawParticles)
}

onMounted(() => {
  const c = canvas.value
  c.width = window.innerWidth
  c.height = window.innerHeight
  ctx = c.getContext("2d")

  initParticles()
  drawParticles()

  window.addEventListener("resize", () => {
    c.width = window.innerWidth
    c.height = window.innerHeight
    initParticles()
  })
})

onBeforeUnmount(() => cancelAnimationFrame(animationFrame))
</script>

<style scoped>
canvas {
  background: transparent;
  pointer-events: none;
}
</style>
