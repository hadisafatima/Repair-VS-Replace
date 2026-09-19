# RepairReplace

RepairReplace is a web application that helps users decide whether a product or appliance is more suitable to **repair or replace** based on its current condition, repair history, repair cost, replacement cost, expected lifespan, and energy usage.

The project consists of a **React frontend** and an **ASP.NET Core Web API backend**. The backend performs the calculations and returns the decision and supporting analysis to the frontend.

---

## What It Does

The application collects information about the product, including:

- Product type
- Current age
- Original purchase price
- Current repair cost
- Number of previous repairs
- Previous repair costs
- Expected years of use after repair
- Replacement cost
- Expected replacement lifespan
- Residual value
- Optional energy consumption and energy price information

Using this information, the backend evaluates:

- Repair cost compared with replacement cost
- Product age relative to its expected lifespan
- Frequency of previous repairs
- Energy efficiency differences
- Long-term repair and replacement costs
- Residual value
- Configurable decision weights and thresholds

It then produces a result such as:

- **Repair**
- **Replace**
- **Borderline**

The result also includes additional calculations such as cost comparisons, break-even information, decision scores, and uncertainty analysis.

---

## Tech Stack

### Frontend

- React
- Vite
- Tailwind CSS
- JavaScript
- Fetch API

### Backend

- ASP.NET Core Web API
- C#
- FluentValidation
- Swagger / OpenAPI
- Dependency Injection

### Configuration

- JSON configuration
- In-memory product and calculation rules
- No database

---

## How the Data Flows

The application follows a simple request-response flow:

```text
User
  │
  ▼
React Frontend
  │
  │  JSON POST Request
  ▼
ASP.NET Core Web API
  │
  ├── Validate Input
  │
  ├── Resolve Product & Calculation Rule
  │
  ├── Calculate Repair Costs
  │
  ├── Calculate Replacement Costs
  │
  ├── Evaluate Decision Factors
  │
  ├── Calculate Decision Score
  │
  └── Perform Uncertainty Analysis
  │
  ▼
JSON Response
  │
  ▼
React Frontend
  │
  ▼
Display Results