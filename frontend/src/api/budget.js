// src/api/budget.js
import api from "../api";  // one level up because api.js is directly in /src

export const getBudgets = () => api.get("/api/budget");
export const compareBudgets = () => api.get("/api/budget/compare");
export const upsertBudget = (payload) => api.post("/api/budget", payload);
