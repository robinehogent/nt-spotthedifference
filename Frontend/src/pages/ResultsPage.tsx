import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { t } from "../translations";

const API_URL = import.meta.env.VITE_API_URL || "";

// Helper function to translate difficulty values from database
const translateDifficulty = (difficulty: string): string => {
  const difficultyKey = difficulty.toLowerCase() as "easy" | "medium" | "hard" | "insane" | "impossible";
  return t(difficultyKey);
};

interface LeaderboardEntry {
  username: string;
  difficulty: string;
  score: number;
  timeSpentSeconds: number;
  datePlayed: string;
}

export default function ResultsPage() {
  const navigate = useNavigate();
  const [results, setResults] = useState<LeaderboardEntry[]>([]);
  const [loading, setLoading] = useState(true);

  const [searchTerm, setSearchTerm] = useState("");

  useEffect(() => {
    fetch(`${API_URL}/api/Game/leaderboard`)
      .then((res) => {
        if (!res.ok) throw new Error("Failed to fetch");
        return res.json();
      })
      .then((data) => setResults(data))
      .catch((err) => console.error(err))
      .finally(() => setLoading(false));
  }, []);

  const filteredResults = results.filter((r) =>
    r.username.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div
      style={{
        padding: "40px",
        fontFamily: "Arial, sans-serif",
        backgroundColor: "#f9f9f9",
        minHeight: "100vh",
      }}
    >
      <div style={{ maxWidth: "800px", margin: "0 auto" }}>
        <h1 style={{ color: "#444", marginBottom: "20px", textAlign: "left" }}>
          {t("resultsOverview")}
        </h1>

        <div
          style={{
            display: "flex",
            justifyContent: "space-between",
            marginBottom: "20px",
          }}
        >
          <button
            onClick={() => navigate("/")}
            style={{
              padding: "10px 20px",
              cursor: "pointer",
              background: "#6c757d",
              color: "white",
              border: "none",
              borderRadius: "5px",
            }}
          >
            ← {t("back")}
          </button>
          <input
            type="text"
            placeholder={t("searchByName")}
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            style={{
              padding: "10px",
              width: "300px",
              borderRadius: "5px",
              border: "1px solid #ccc",
              outline: "none",
            }}
          />
        </div>

        {loading ? (
          <p style={{ color: "#666" }}>{t("loadingData")}</p>
        ) : (
          <div
            style={{
              borderRadius: "8px",
              overflow: "hidden",
              border: "1px solid #ddd",
              background: "white",
            }}
          >
            <table style={{ width: "100%", borderCollapse: "collapse" }}>
              <thead
                style={{
                  background: "#f1f1f1",
                  color: "#333",
                  borderBottom: "2px solid #ddd",
                }}
              >
                <tr>
                  <th style={thStyle}>{t("name")}</th>
                  <th style={thStyle}>{t("level")}</th>
                  <th style={thStyle}>{t("score")}</th>
                  <th style={thStyle}>{t("timeSeconds")}</th>
                  <th style={thStyle}>{t("date")}</th>
                </tr>
              </thead>
              <tbody>
                {filteredResults.map((r, index) => (
                  <tr key={index} style={{ borderBottom: "1px solid #eee" }}>
                    <td
                      style={{ ...tdStyle, fontWeight: "bold", color: "#333" }}
                    >
                      {r.username}
                    </td>
                    <td style={tdStyle}>{translateDifficulty(r.difficulty)}</td>
                    <td style={tdStyle}>{r.score}</td>
                    <td style={tdStyle}>{r.timeSpentSeconds}</td>
                    <td style={{ ...tdStyle, color: "#666" }}>
                      {new Date(r.datePlayed).toLocaleDateString()}{" "}
                      {new Date(r.datePlayed).toLocaleTimeString([], {
                        hour: "2-digit",
                        minute: "2-digit",
                      })}
                    </td>
                  </tr>
                ))}
                {filteredResults.length === 0 && (
                  <tr>
                    <td
                      colSpan={5}
                      style={{
                        padding: 20,
                        textAlign: "center",
                        color: "#888",
                      }}
                    >
                      {t("noResultsFoundPeriod")}
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  );
}

const thStyle: React.CSSProperties = {
  padding: "15px",
  textAlign: "left",
  fontSize: "0.95rem",
  fontWeight: "600",
};
const tdStyle: React.CSSProperties = {
  padding: "12px",
  textAlign: "left",
  fontSize: "0.95rem",
};
