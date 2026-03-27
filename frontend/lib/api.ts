// ---Made By Destiny7 Softwares---
import { alerts as mockAlerts, customers as mockCustomers, dashboard as mockDashboard } from "@/lib/mock-data";

const apiBaseUrl = process.env.NEXT_PUBLIC_API_BASE_URL ?? "http://localhost:8080";

type ApiDashboard = {
  totalCustomers: number;
  portfolioAverageRisk: number;
  highRiskCustomers: number;
  predictedChurnPercent: number;
  predictedLatePaymentPercent: number;
  revenueAtRisk: number;
};

type ApiCustomerListItem = {
  id: string;
  name: string;
  segment: string;
  industry: string;
  country: string;
  currentPlan: string;
  overallRiskScore: number;
  churnRiskScore: number;
  latePaymentRiskScore: number;
  monthlyRevenue: number;
  riskLevel: string;
};

type ApiCustomerDetail = {
  customer: {
    id: string;
    name: string;
    segment: string;
    country: string;
    industry: string;
    monthlyRevenue: number;
    currentPlan: string;
  };
  latestRiskAssessment: {
    overallRiskScore: number;
    churnRiskScore: number;
    latePaymentRiskScore: number;
    summary: string;
  };
  payments: Array<{
    dueDate: string;
    status: string;
    amount: number;
  }>;
  tickets: Array<{
    severity: string;
  }>;
  usageMetrics: Array<{
    activeUsers: number;
    usageVariationPercent: number;
  }>;
};

type ApiAlert = {
  id: string;
  customerId: string;
  customerName: string;
  type: string;
  severity: string;
  title: string;
  description: string;
  isResolved: boolean;
};

type ApiCopilot = {
  customerId: string;
  question: string;
  answer: string;
  model: string;
  generatedAt: string;
  analysis: {
    executiveSummary: string;
    riskLevel: string;
    confidence: number;
    topSignals: Array<{ label: string; value: string; impact: string }>;
    recommendedActions: string[];
    followUpQuestions: string[];
  };
  knowledge: Array<{
    title: string;
    sourceType: string;
    path: string;
    snippet: string;
    score: number;
  }>;
};

async function safeFetch<T>(path: string): Promise<T | null> {
  try {
    const response = await fetch(`${apiBaseUrl}${path}`, {
      next: { revalidate: 60 }
    });

    if (!response.ok) {
      return null;
    }

    return (await response.json()) as T;
  } catch {
    return null;
  }
}

export async function getDashboardViewModel() {
  const apiDashboard = await safeFetch<ApiDashboard>("/api/risk-assessments");
  if (!apiDashboard) {
    return mockDashboard;
  }

  return {
    ...mockDashboard,
    portfolioRiskScore: Math.round(apiDashboard.portfolioAverageRisk),
    highRiskClients: apiDashboard.highRiskCustomers,
    projectedChurn: `${Math.round(apiDashboard.predictedChurnPercent)}%`,
    latePayments: Math.round(apiDashboard.predictedLatePaymentPercent * 4.4),
    revenueAtRisk: new Intl.NumberFormat("en-US", {
      style: "currency",
      currency: "USD",
      maximumFractionDigits: 0
    }).format(apiDashboard.revenueAtRisk)
  };
}

export async function getCustomersViewModel() {
  const apiCustomers = await safeFetch<ApiCustomerListItem[]>("/api/customers");
  if (!apiCustomers) {
    return mockCustomers;
  }

  return apiCustomers.map((customer) => ({
    id: customer.id,
    name: customer.name,
    segment: customer.segment,
    country: customer.country,
    industry: customer.industry,
    plan: customer.currentPlan,
    risk: Math.round(customer.overallRiskScore),
    churn: Math.round(customer.churnRiskScore),
    late: Math.round(customer.latePaymentRiskScore),
    revenue: new Intl.NumberFormat("en-US", {
      style: "currency",
      currency: "USD",
      maximumFractionDigits: 0
    }).format(customer.monthlyRevenue),
    summary: `${customer.segment} account with ${customer.riskLevel.toLowerCase()} monitored risk profile.`,
    countryFlag: customer.country.slice(0, 2).toUpperCase(),
    status:
      customer.overallRiskScore >= 75
        ? "Payment Overdue"
        : customer.overallRiskScore >= 45
          ? "Renewal Watch"
          : "Healthy",
    contractRenewal: customer.overallRiskScore >= 45 ? "60 days" : "120 days",
    activeUsers: Math.max(45, Math.round(customer.monthlyRevenue / 520)),
    usageDelta: customer.overallRiskScore >= 75 ? "-28%" : customer.overallRiskScore >= 45 ? "-7%" : "+5%",
    aiNarrative: `${customer.name} remains under monitoring due to the current balance between product adoption, billing behavior, and revenue exposure.`,
    paymentHistory: [
      { month: "Jan", status: "Paid", amount: new Intl.NumberFormat("en-US", { style: "currency", currency: "USD", maximumFractionDigits: 0 }).format(customer.monthlyRevenue) },
      { month: "Feb", status: "Paid", amount: new Intl.NumberFormat("en-US", { style: "currency", currency: "USD", maximumFractionDigits: 0 }).format(customer.monthlyRevenue) },
      { month: "Mar", status: customer.overallRiskScore >= 75 ? "Overdue" : "Paid", amount: new Intl.NumberFormat("en-US", { style: "currency", currency: "USD", maximumFractionDigits: 0 }).format(customer.monthlyRevenue) }
    ],
    recentSignals: [
      { label: "Critical tickets", value: customer.overallRiskScore >= 75 ? "4" : customer.overallRiskScore >= 45 ? "1" : "0", tone: customer.overallRiskScore >= 75 ? "red" : customer.overallRiskScore >= 45 ? "amber" : "green" },
      { label: "Usage delta", value: customer.overallRiskScore >= 75 ? "-28%" : customer.overallRiskScore >= 45 ? "-7%" : "+5%", tone: customer.overallRiskScore >= 75 ? "orange" : customer.overallRiskScore >= 45 ? "amber" : "green" },
      { label: "Renewal window", value: customer.overallRiskScore >= 45 ? "60 days" : "120 days", tone: customer.overallRiskScore >= 75 ? "orange" : customer.overallRiskScore >= 45 ? "amber" : "green" }
    ]
  }));
}

export async function getCustomerDetailViewModel(id: string) {
  const [apiCustomer, customerList] = await Promise.all([
    safeFetch<ApiCustomerDetail>(`/api/customers/${id}`),
    getCustomersViewModel()
  ]);

  if (!apiCustomer) {
    return customerList.find((item) => item.id === id) ?? customerList[0];
  }

  const fallback = customerList.find((item) => item.id === id) ?? customerList[0];
  return {
    ...fallback,
    id: apiCustomer.customer.id,
    name: apiCustomer.customer.name,
    segment: apiCustomer.customer.segment,
    country: apiCustomer.customer.country,
    industry: apiCustomer.customer.industry,
    plan: apiCustomer.customer.currentPlan,
    risk: Math.round(apiCustomer.latestRiskAssessment.overallRiskScore),
    churn: Math.round(apiCustomer.latestRiskAssessment.churnRiskScore),
    late: Math.round(apiCustomer.latestRiskAssessment.latePaymentRiskScore),
    revenue: new Intl.NumberFormat("en-US", {
      style: "currency",
      currency: "USD",
      maximumFractionDigits: 0
    }).format(apiCustomer.customer.monthlyRevenue),
    summary: apiCustomer.latestRiskAssessment.summary,
    activeUsers: apiCustomer.usageMetrics[0]?.activeUsers ?? fallback.activeUsers,
    usageDelta: `${apiCustomer.usageMetrics[0]?.usageVariationPercent ?? 0}%`,
    aiNarrative: apiCustomer.latestRiskAssessment.summary,
    paymentHistory: apiCustomer.payments.slice(0, 3).map((payment, index) => ({
      month: ["Jan", "Feb", "Mar"][index] ?? `P${index + 1}`,
      status: payment.status,
      amount: new Intl.NumberFormat("en-US", {
        style: "currency",
        currency: "USD",
        maximumFractionDigits: 0
      }).format(payment.amount)
    })),
    recentSignals: [
      {
        label: "Critical tickets",
        value: String(apiCustomer.tickets.filter((ticket) => ticket.severity === "Critical").length),
        tone: apiCustomer.tickets.filter((ticket) => ticket.severity === "Critical").length >= 3 ? "red" : "amber"
      },
      {
        label: "Usage delta",
        value: `${apiCustomer.usageMetrics[0]?.usageVariationPercent ?? 0}%`,
        tone: (apiCustomer.usageMetrics[0]?.usageVariationPercent ?? 0) < -15 ? "orange" : "green"
      },
      {
        label: "Renewal window",
        value: fallback.contractRenewal,
        tone: fallback.recentSignals[2]?.tone ?? "amber"
      }
    ]
  };
}

export async function getAlertsViewModel() {
  const apiAlerts = await safeFetch<ApiAlert[]>("/api/alerts");
  if (!apiAlerts) {
    return mockAlerts;
  }

  return apiAlerts.map((alert) => ({
    id: alert.id,
    severity: alert.severity,
    title: alert.title,
    customer: alert.customerName,
    type: alert.type,
    status: alert.isResolved ? "Resolved" : "Open"
  }));
}

export async function getCustomerCopilotViewModel(id: string) {
  const apiCopilot = await safeFetch<ApiCopilot>(`/api/ai/copilot/customer/${id}`);
  if (!apiCopilot) {
    return {
      question: "What is driving current risk and what should leadership do next?",
      answer:
        "Current risk is being driven by a compounding pattern of payment delay, weakening adoption, and critical support volume. Leadership should run a coordinated collections, product recovery, and renewal-containment plan.",
      model: "fallback-structured-copilot",
      analysis: {
        executiveSummary:
          "The account is in a fragile state because billing pressure, product adoption decline, and service instability are reinforcing each other.",
        riskLevel: "Critical",
        confidence: 0.84,
        topSignals: [
          { label: "Payment delay", value: "12 days", impact: "high" },
          { label: "Usage variation", value: "-32%", impact: "high" },
          { label: "Critical tickets", value: "4", impact: "high" }
        ],
        recommendedActions: [
          "Coordinate collections and customer success outreach within 24 hours.",
          "Stabilize unresolved critical incidents before renewal negotiation.",
          "Create an executive recovery plan tied to adoption improvement."
        ],
        followUpQuestions: [
          "Which support themes are hurting stakeholder trust most?",
          "Should we escalate this account into a renewal war room?"
        ]
      },
      knowledge: [
        {
          title: "alpha-capital-renewal-playbook",
          sourceType: "contract",
          snippet: "Trigger executive review if platform adoption falls more than 20 percent over a rolling 90-day window."
        },
        {
          title: "alpha-capital-support-patterns",
          sourceType: "ticket",
          snippet: "Repeated critical incidents correlate with declining stakeholder confidence and reduced weekly active usage."
        }
      ]
    };
  }

  return {
    question: apiCopilot.question,
    answer: apiCopilot.answer,
    model: apiCopilot.model,
    analysis: apiCopilot.analysis,
    knowledge: apiCopilot.knowledge
  };
}
