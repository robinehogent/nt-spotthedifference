import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { t } from "../translations";

interface LeaderboardEntry {
  username: string;
  difficulty: string;
  score: number;
  timeSpentSeconds: number;
}

const API_URL = import.meta.env.VITE_API_URL || "";

// Helper function to translate difficulty values from database
const translateDifficulty = (difficulty: string): string => {
  const difficultyKey = difficulty.toLowerCase() as "easy" | "medium" | "hard" | "insane" | "impossible";
  return t(difficultyKey);
};

export default function StartMenu() {
  const navigate = useNavigate();

  const [username, setUsername] = useState(() => {
    return localStorage.getItem("username") || "";
  });

  // NEW: language state, default from localStorage or "en"
  const [language, setLanguage] = useState<string>(() => {
    return localStorage.getItem("language") || "en";
  });

  const [showLeaderboard, setShowLeaderboard] = useState(false);
  const [leaders, setLeaders] = useState<LeaderboardEntry[]>([]);
  const [loading, setLoading] = useState(false);

  const [searchTerm, setSearchTerm] = useState("");
  const [isAuthorized, setIsAuthorized] = useState(false);
  const [passwordInput, setPasswordInput] = useState("");

  const handleChooseDifficulty = (difficultyId: number) => {
    const finalName = username.trim() || t("guest");
    localStorage.setItem("username", finalName);
    localStorage.setItem("difficultyId", difficultyId.toString());

    // NEW: persist language so other pages can use it
    localStorage.setItem("language", language);

    navigate(`/level/${difficultyId}/start`);
  };

  const openLeaderboardModal = () => {
    setShowLeaderboard(true);
    setIsAuthorized(false);
    setPasswordInput("");
    setLeaders([]);
    setSearchTerm("");
  };

  const handlePasswordSubmit = () => {
    if (passwordInput === "1111") {
      setIsAuthorized(true);
      loadLeaderboardData();
    } else {
      alert(t("wrongPassword"));
    }
  };

  const loadLeaderboardData = async () => {
    setLoading(true);
    try {
      const res = await fetch(`${API_URL}/api/Game/leaderboard`);
      if (res.ok) {
        const data = await res.json();
        setLeaders(data);
      }
    } catch (error) {
      console.error("Failed to load leaderboard", error);
    } finally {
      setLoading(false);
    }
  };

  const filteredLeaders = leaders.filter((entry) =>
    entry.username.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div
      style={{
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        justifyContent: "center",
        height: "100vh",
        backgroundColor: "#f0f4f8",
        fontFamily: "Arial, sans-serif",
      }}
    >
      <h1 style={{ fontSize: "3rem", color: "#333", marginBottom: "40px" }}>
        {t("appTitle")}
      </h1>

      {/* Language selector top-center or near title */}
      <div style={{ position: "absolute", top: 20, margin: "0 auto" }}>
        <select
          value={language}
          onChange={(e) => {
            const lang = e.target.value;
            setLanguage(lang);
            localStorage.setItem("language", lang);
          }}
          style={{
            padding: "6px 10px",
            borderRadius: "6px",
            border: "1px solid #ccc",
            fontSize: "0.9rem",
          }}
        >
          <option value="en">{t("english")}</option>
          <option value="nl">{t("dutch")}</option>
          <option value="fr">{t("french")}</option>
        </select>
      </div>

      <button
        onClick={openLeaderboardModal}
        style={{
          position: "absolute",
          top: "20px",
          left: "20px",
          background: "white",
          border: "1px solid #ccc",
          borderRadius: "5px",
          padding: "8px 15px",
          cursor: "pointer",
          color: "#333",
          fontWeight: "bold",
          display: "flex",
          alignItems: "center",
          gap: "5px",
        }}
      >
        {t("leaderboard")}
      </button>

      <button
        onClick={() => navigate("/admin")}
        style={{
          position: "absolute",
          top: "20px",
          right: "20px",
          background: "transparent",
          border: "1px solid #ccc",
          borderRadius: "5px",
          padding: "8px 15px",
          cursor: "pointer",
          color: "#333",
          fontWeight: "bold",
        }}
      >
        {t("admin")}
      </button>

      <div
        style={{
          backgroundColor: "white",
          padding: "40px",
          borderRadius: "20px",
          boxShadow: "0 4px 15px rgba(0,0,0,0.1)",
          display: "flex",
          flexDirection: "column",
          gap: "20px",
          width: "300px",
        }}
      >
        <label style={{ fontSize: "1.2rem", color: "#555" }}>
          {t("username")}
        </label>

        <input
          type="text"
          placeholder={t("usernamePlaceholder")}
          value={username}
          onChange={(e) => setUsername(e.target.value)}
          style={{
            padding: "15px",
            fontSize: "1.1rem",
            borderRadius: "10px",
            border: "1px solid #ccc",
            outline: "none",
          }}
        />

        <div
          style={{
            display: "flex",
            flexDirection: "column",
            gap: "12px",
            marginTop: "10px",
          }}
        >
          <button onClick={() => handleChooseDifficulty(1)} style={btnStyle}>
            {t("easy")}
          </button>
          <button onClick={() => handleChooseDifficulty(2)} style={btnStyle}>
            {t("medium")}
          </button>
          <button onClick={() => handleChooseDifficulty(3)} style={btnStyle}>
            {t("hard")}
          </button>
          <button onClick={() => handleChooseDifficulty(4)} style={btnStyle}>
            {t("insane")}
          </button>
          <button onClick={() => handleChooseDifficulty(5)} style={btnStyle}>
            {t("impossible")}
          </button>
        </div>
      </div>

      {showLeaderboard && (
        <div style={modalOverlayStyle}>
          <div style={modalContentStyle}>
            <div
              style={{
                display: "flex",
                justifyContent: "space-between",
                alignItems: "center",
                marginBottom: "20px",
              }}
            >
              <h2 style={{ margin: 0, color: "#333" }}>
                {t("resultsOverview")}
              </h2>
              <button
                onClick={() => setShowLeaderboard(false)}
                style={closeBtnStyle}
              >
                ✕
              </button>
            </div>

            {!isAuthorized ? (
              <div style={{ textAlign: "center", padding: "20px 0" }}>
                <p style={{ marginBottom: 10, color: "#555" }}>
                  {t("enterPassword")}
                </p>
                <input
                  type="password"
                  value={passwordInput}
                  onChange={(e) => setPasswordInput(e.target.value)}
                  onKeyDown={(e) => e.key === "Enter" && handlePasswordSubmit()}
                  placeholder={t("passwordPlaceholder")}
                  style={{
                    padding: "10px",
                    borderRadius: "5px",
                    border: "1px solid #ccc",
                    fontSize: "1rem",
                    width: "70%",
                    marginBottom: "15px",
                    textAlign: "center",
                  }}
                />
                <br />
                <button
                  onClick={handlePasswordSubmit}
                  style={{ ...btnStyle, padding: "8px 20px", fontSize: "1rem" }}
                >
                  {t("view")}
                </button>
              </div>
            ) : (
              <>
                <div style={{ marginBottom: "15px" }}>
                  <input
                    type="text"
                    placeholder={t("searchByName")}
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                    style={{
                      width: "100%",
                      padding: "10px",
                      borderRadius: "5px",
                      border: "1px solid #ccc",
                      boxSizing: "border-box",
                    }}
                  />
                </div>

                {loading ? (
                  <p>{t("loadingResults")}</p>
                ) : (
                  <div
                    style={{
                      maxHeight: "50vh",
                      overflowY: "auto",
                      border: "1px solid #fffdfd",
                      borderRadius: "5px",
                    }}
                  >
                    <table
                      style={{
                        width: "100%",
                        borderCollapse: "collapse",
                        fontSize: "0.9rem",
                      }}
                    >
                      <thead
                        style={{
                          background: "#f9f9f9",
                          position: "sticky",
                          top: 0,
                        }}
                      >
                        <tr>
                          <th style={thStyle}>{t("name")}</th>
                          <th style={thStyle}>{t("level")}</th>
                          <th style={thStyle}>{t("score")}</th>
                          <th style={thStyle}>{t("time")}</th>
                        </tr>
                      </thead>
                      <tbody>
                        {filteredLeaders.map((entry, i) => (
                          <tr
                            key={i}
                            style={{ borderBottom: "1px solid #eee" }}
                          >
                            <td
                              style={{
                                ...tdStyle,
                                fontWeight: "bold",
                              }}
                            >
                              {entry.username}
                            </td>
                            <td style={tdStyle}>{translateDifficulty(entry.difficulty)}</td>
                            <td
                              style={{
                                ...tdStyle,
                                fontWeight: "bold",
                                color: "green",
                              }}
                            >
                              {entry.score}
                            </td>
                            <td style={tdStyle}>{entry.timeSpentSeconds}s</td>
                          </tr>
                        ))}
                        {filteredLeaders.length === 0 && (
                          <tr>
                            <td
                              colSpan={4}
                              style={{
                                padding: 20,
                                textAlign: "center",
                                color: "#999",
                              }}
                            >
                              {t("noResultsFound")}
                            </td>
                          </tr>
                        )}
                      </tbody>
                    </table>
                  </div>
                )}
              </>
            )}

            <button
              onClick={() => setShowLeaderboard(false)}
              style={{
                ...btnStyle,
                marginTop: "20px",
                padding: "10px",
                width: "100%",
                background: "#6c757d",
              }}
            >
              {t("close")}
            </button>
          </div>
        </div>
      )}
    </div>
  );
}

const btnStyle: React.CSSProperties = {
  padding: "15px",
  fontSize: "1.1rem",
  fontWeight: "bold",
  color: "white",
  backgroundColor: "#4CAF50",
  border: "none",
  borderRadius: "10px",
  cursor: "pointer",
  transition: "background 0.3s",
};

const modalOverlayStyle: React.CSSProperties = {
  position: "fixed",
  top: 0,
  left: 0,
  width: "100%",
  height: "100%",
  backgroundColor: "rgba(0,0,0,0.5)",
  display: "flex",
  justifyContent: "center",
  alignItems: "center",
  zIndex: 1000,
};

const modalContentStyle: React.CSSProperties = {
  backgroundColor: "white",
  padding: "30px",
  borderRadius: "15px",
  width: "90%",
  maxWidth: "600px",
  boxShadow: "0 2px 10px rgba(0,0,0,0.2)",
  textAlign: "left",
};

const closeBtnStyle: React.CSSProperties = {
  background: "transparent",
  border: "none",
  fontSize: "1.5rem",
  cursor: "pointer",
  color: "#999",
};

const thStyle: React.CSSProperties = {
  padding: "12px",
  borderBottom: "2px solid #ddd",
  color: "#555",
  fontWeight: "600",
  textAlign: "left",
};
const tdStyle: React.CSSProperties = {
  padding: "12px",
  color: "#333",
  textAlign: "left",
};
