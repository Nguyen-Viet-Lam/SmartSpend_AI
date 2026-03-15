(() => {
    const css = getComputedStyle(document.documentElement);
    const primary = (css.getPropertyValue("--primary") || "#007aff").trim();
    const primaryDeep = (css.getPropertyValue("--primary-deep") || "#005fcc").trim();
    const accent = (css.getPropertyValue("--accent") || "#ff9500").trim();
    const ok = (css.getPropertyValue("--ok") || "#1f8f5f").trim();

    const marker = document.querySelector("[data-dashboard-ready]");
    const balanceEl = document.querySelector("[data-kpi-balance]");
    const expenseEl = document.querySelector("[data-kpi-expense]");
    const incomeEl = document.querySelector("[data-kpi-income]");
    const forecastEl = document.querySelector("[data-kpi-forecast]");
    const pieLegendEl = document.querySelector("[data-pie-legend]");

    const pieCanvas = document.getElementById("spendingPieChart");
    const barCanvas = document.getElementById("incomeExpenseBarChart");
    const storage = window.smartSpendStorage;

    if (!marker || !balanceEl || !expenseEl || !incomeEl || !forecastEl || !storage) {
        return;
    }

    let pieChart = null;
    let barChart = null;

    const currency = (value) => `${Math.round(Number(value || 0)).toLocaleString("vi-VN")} d`;
    const inLast7Days = (dateStr, referenceNow) => {
        const date = new Date(dateStr);
        const diff = referenceNow.getTime() - date.getTime();
        return diff >= 0 && diff <= 7 * 24 * 60 * 60 * 1000;
    };

    const buildCategorySeries = (transactions) => {
        const categoryMap = new Map();
        transactions
            .filter((x) => x.type === "Expense")
            .forEach((item) => {
                const category = item.category || "Khac";
                const amount = Number(item.amount || 0);
                categoryMap.set(category, (categoryMap.get(category) || 0) + amount);
            });

        if (!categoryMap.size) {
            return {
                labels: ["An uong", "Di chuyen", "Giai tri", "Khac"],
                values: [35, 20, 15, 30]
            };
        }

        return {
            labels: Array.from(categoryMap.keys()),
            values: Array.from(categoryMap.values())
        };
    };

    const buildMonthlySeries = (transactions, referenceNow) => {
        const monthLabels = [];
        const incomeData = [];
        const expenseData = [];

        for (let i = 5; i >= 0; i--) {
            const d = new Date(referenceNow.getFullYear(), referenceNow.getMonth() - i, 1);
            const month = d.getMonth();
            const year = d.getFullYear();
            monthLabels.push(`T${month + 1}`);

            const inMonth = transactions.filter((x) => {
                const txDate = new Date(x.occurredAt);
                return txDate.getMonth() === month && txDate.getFullYear() === year;
            });

            incomeData.push(inMonth.filter((x) => x.type === "Income").reduce((sum, item) => sum + Number(item.amount || 0), 0));
            expenseData.push(inMonth.filter((x) => x.type === "Expense").reduce((sum, item) => sum + Number(item.amount || 0), 0));
        }

        return { labels: monthLabels, incomeData, expenseData };
    };

    const renderCharts = (categorySeries, monthSeries) => {
        if (!window.Chart || !pieCanvas || !barCanvas) {
            marker.textContent += " - Chart.js chua tai duoc.";
            return;
        }

        if (pieChart) {
            pieChart.destroy();
        }

        if (barChart) {
            barChart.destroy();
        }

        if (pieLegendEl) {
            pieLegendEl.innerHTML = categorySeries.labels
                .map((label, index) => `<span>${label} ${Math.round(categorySeries.values[index] || 0).toLocaleString("vi-VN")}</span>`)
                .join("");
        }

        pieChart = new Chart(pieCanvas, {
            type: "pie",
            data: {
                labels: categorySeries.labels,
                datasets: [
                    {
                        data: categorySeries.values,
                        backgroundColor: [primary, accent, primaryDeep, "#9aa8b7", "#c65f9a", "#27a1a1"]
                    }
                ]
            },
            options: {
                plugins: {
                    legend: { display: false }
                },
                maintainAspectRatio: false
            }
        });

        barChart = new Chart(barCanvas, {
            type: "bar",
            data: {
                labels: monthSeries.labels,
                datasets: [
                    {
                        label: "Thu nhap",
                        data: monthSeries.incomeData,
                        backgroundColor: ok
                    },
                    {
                        label: "Chi tieu",
                        data: monthSeries.expenseData,
                        backgroundColor: accent
                    }
                ]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    y: {
                        ticks: {
                            callback: (value) => Number(value).toLocaleString("vi-VN")
                        }
                    }
                }
            }
        });
    };

    const render = () => {
        const now = new Date();
        const wallets = storage.read("smartspend-wallets", []) || [];
        const transactions = storage.read("smartspend-transactions", []) || [];

        const totalBalance = wallets.reduce((sum, item) => sum + Number(item.balance || 0), 0);
        const last7Expense = transactions
            .filter((x) => x.type === "Expense" && inLast7Days(x.occurredAt, now))
            .reduce((sum, item) => sum + Number(item.amount || 0), 0);
        const last7Income = transactions
            .filter((x) => x.type === "Income" && inLast7Days(x.occurredAt, now))
            .reduce((sum, item) => sum + Number(item.amount || 0), 0);

        const monthStart = new Date(now.getFullYear(), now.getMonth(), 1);
        const spentThisMonth = transactions
            .filter((x) => x.type === "Expense" && new Date(x.occurredAt) >= monthStart)
            .reduce((sum, item) => sum + Number(item.amount || 0), 0);

        const daysPassed = Math.max(1, now.getDate());
        const daysInMonth = new Date(now.getFullYear(), now.getMonth() + 1, 0).getDate();
        const forecast = (spentThisMonth / daysPassed) * daysInMonth;

        balanceEl.textContent = currency(totalBalance);
        expenseEl.textContent = currency(last7Expense);
        incomeEl.textContent = currency(last7Income);
        forecastEl.textContent = currency(forecast || 0);
        marker.textContent = `Dashboard cap nhat luc ${new Date().toLocaleTimeString("vi-VN")}`;

        renderCharts(buildCategorySeries(transactions), buildMonthlySeries(transactions, now));
    };

    window.addEventListener("smartspend:wallets-changed", render);
    window.addEventListener("smartspend:transactions-changed", render);
    window.addEventListener("storage", render);
    render();
})();
