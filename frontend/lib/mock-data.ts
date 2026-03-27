// ---Made By Destiny7 Softwares---
export const dashboard = {
  portfolioRiskScore: 72,
  portfolioRiskLabel: "High Risk",
  highRiskClients: 47,
  highRiskLabel: "Critical",
  projectedChurn: "32%",
  projectedChurnLabel: "Next 30 Days",
  latePayments: 128,
  latePaymentsLabel: "Overdue",
  revenueAtRisk: "$489K",
  revenueAtRiskLabel: "At Risk",
  topDrivers: [32, 35, 36, 48, 52, 41, 44, 53, 45, 43, 54, 54, 38, 39, 47, 46],
  trend: [
    { label: "W1", churn: 41, late: 38, revenue: 16 },
    { label: "W2", churn: 43, late: 40, revenue: 14 },
    { label: "W3", churn: 45, late: 47, revenue: 18 },
    { label: "W4", churn: 42, late: 36, revenue: 12 },
    { label: "W5", churn: 48, late: 44, revenue: 15 },
    { label: "W6", churn: 51, late: 50, revenue: 16 },
    { label: "W7", churn: 44, late: 39, revenue: 20 },
    { label: "W8", churn: 52, late: 46, revenue: 17 },
    { label: "W9", churn: 57, late: 55, revenue: 19 },
    { label: "W10", churn: 46, late: 41, revenue: 18 },
    { label: "W11", churn: 42, late: 37, revenue: 22 },
    { label: "W12", churn: 54, late: 49, revenue: 16 },
    { label: "W13", churn: 56, late: 53, revenue: 18 },
    { label: "W14", churn: 44, late: 39, revenue: 20 },
    { label: "W15", churn: 53, late: 48, revenue: 17 },
    { label: "W16", churn: 58, late: 56, revenue: 18 }
  ],
  highRiskTable: [
    { name: "Alpha Corp", score: 92, churn: "High", status: "Payment Overdue", tone: "amber" },
    { name: "Beta Industries", score: 88, churn: "Critical", status: "High Churn Risk", tone: "red" },
    { name: "Delta Solutions", score: 84, churn: "High", status: "Usage Decline", tone: "orange" },
    { name: "Omega Holdings", score: 79, churn: "Medium", status: "Contract Expiring", tone: "green" },
    { name: "Gamma Inc.", score: 76, churn: "High", status: "Support Escalations", tone: "red" }
  ],
  keyAlerts: [
    "Major Account Downgrade - Alpha Corp",
    "15+ Days Overdue - Client Beta",
    "Usage Drop 40% - Delta Solutions",
    "Multiple Critical Tickets - Gamma Inc."
  ],
  churnGauge: 68,
  churnGaugeLabel: "High",
  latePaymentForecast: "87%",
  latePaymentForecastLabel: "Likely Delinquent",
  potentialLoss: "$154,000",
  simulator: {
    paymentDelay: "15 days",
    usageDecline: "-45%",
    criticalTickets: "4",
    outcome: "High Risk of Churn & Delinquency",
    score: 85
  }
};

export const dashboardChrome = {
  userName: "Admin",
  productSubtitle: "Financial Risk Intelligence Platform",
  sideNav: [
    { label: "Overview", short: "O" },
    { label: "Dashboard", short: "D", active: true },
    { label: "Clients", short: "C" },
    { label: "Reports", short: "R" },
    { label: "Integrations", short: "I" },
    { label: "Downloads", short: "L" },
    { label: "Insights", short: "S" }
  ]
};

export const customers = [
  {
    id: "f0b50a84-cad7-4dcc-9f42-16fd5de2f101",
    name: "Alpha Capital Partners",
    segment: "Enterprise",
    country: "United States",
    industry: "Fintech",
    plan: "Enterprise Pulse",
    risk: 81,
    churn: 79,
    late: 84,
    revenue: "$182k",
    summary: "Usage down 32%, 4 critical tickets, invoice overdue by 12 days.",
    countryFlag: "US",
    status: "Payment Overdue",
    contractRenewal: "45 days",
    activeUsers: 350,
    usageDelta: "-32%",
    aiNarrative: "Customer shows elevated churn and payment risk due to sharp platform usage decline, repeated critical support incidents, recent payment delay, and an active renewal pressure window.",
    paymentHistory: [
      { month: "Jan", status: "Paid", amount: "$182k" },
      { month: "Feb", status: "Paid", amount: "$182k" },
      { month: "Mar", status: "Overdue", amount: "$182k" }
    ],
    recentSignals: [
      { label: "Critical tickets", value: "4", tone: "red" },
      { label: "Usage delta", value: "-32%", tone: "orange" },
      { label: "Renewal window", value: "45 days", tone: "amber" }
    ]
  },
  {
    id: "f0b50a84-cad7-4dcc-9f42-16fd5de2f102",
    name: "Nova Retail Cloud",
    segment: "Mid-Market",
    country: "Brazil",
    industry: "Retail Tech",
    plan: "Growth Shield",
    risk: 28,
    churn: 22,
    late: 19,
    revenue: "$76k",
    summary: "Healthy usage, stable billing profile and low support pressure.",
    countryFlag: "BR",
    status: "Healthy",
    contractRenewal: "120 days",
    activeUsers: 188,
    usageDelta: "+6%",
    aiNarrative: "Customer remains in a low-risk band with stable collections, resilient product usage, and no active support escalations.",
    paymentHistory: [
      { month: "Jan", status: "Paid", amount: "$76k" },
      { month: "Feb", status: "Paid", amount: "$76k" },
      { month: "Mar", status: "Paid", amount: "$76k" }
    ],
    recentSignals: [
      { label: "Critical tickets", value: "0", tone: "green" },
      { label: "Usage delta", value: "+6%", tone: "green" },
      { label: "Renewal window", value: "120 days", tone: "green" }
    ]
  },
  {
    id: "f0b50a84-cad7-4dcc-9f42-16fd5de2f103",
    name: "Orbit Manufacturing Hub",
    segment: "Enterprise",
    country: "Germany",
    industry: "Industrial SaaS",
    plan: "Enterprise Pulse",
    risk: 46,
    churn: 41,
    late: 38,
    revenue: "$129k",
    summary: "Moderate renewal exposure with mild adoption softness.",
    countryFlag: "DE",
    status: "Renewal Watch",
    contractRenewal: "63 days",
    activeUsers: 265,
    usageDelta: "-4%",
    aiNarrative: "Moderate revenue exposure is driven by upcoming renewal timing and softer product adoption, but billing risk remains contained.",
    paymentHistory: [
      { month: "Jan", status: "Paid", amount: "$129k" },
      { month: "Feb", status: "Paid", amount: "$129k" },
      { month: "Mar", status: "Paid", amount: "$129k" }
    ],
    recentSignals: [
      { label: "Critical tickets", value: "1", tone: "amber" },
      { label: "Usage delta", value: "-4%", tone: "amber" },
      { label: "Renewal window", value: "63 days", tone: "orange" }
    ]
  }
];

export const alerts = [
  { id: "1", severity: "Critical", title: "12-day overdue invoice", customer: "Alpha Capital Partners", type: "Late Payment", status: "Open" },
  { id: "2", severity: "High", title: "Adoption dropped below threshold", customer: "Alpha Capital Partners", type: "Churn", status: "Open" },
  { id: "3", severity: "Warning", title: "Renewal window opened", customer: "Orbit Manufacturing Hub", type: "Renewal", status: "Open" }
];

export const alertStats = [
  { label: "Open alerts", value: "128", tone: "red" },
  { label: "Critical cases", value: "34", tone: "orange" },
  { label: "Resolved today", value: "19", tone: "green" }
];

export const simulatorPresets = [
  { label: "Payment Delay", value: "15 days" },
  { label: "Usage Decline", value: "-45%" },
  { label: "Critical Tickets", value: "4" },
  { label: "Renewal Window", value: "30 days" }
];

export const uploadCards = [
  { label: "Customers", value: "3,842 rows", tone: "green" },
  { label: "Payments", value: "14,280 rows", tone: "blue" },
  { label: "Usage", value: "11,906 rows", tone: "orange" },
  { label: "Tickets", value: "5,114 rows", tone: "red" }
];
