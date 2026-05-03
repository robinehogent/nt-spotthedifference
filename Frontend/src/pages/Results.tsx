import { useNavigate } from "react-router-dom";
import { t } from "../translations";

export default function Results() {
  const navigate = useNavigate();
  const playerName =
    localStorage.getItem("playerName") ||
    localStorage.getItem("username") ||
    t("guest");

  const handleRestart = () => {
    navigate("/");
  };

  return (
    <div
      style={{
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        justifyContent: "center",
        height: "100vh",
        backgroundColor: "#e8f5e9",
        fontFamily: "Arial, sans-serif",
      }}
    >
      <h1 style={{ fontSize: "3rem", color: "#2e7d32", marginBottom: "10px" }}>
        {t("congratulations")}
      </h1>

      <h2 style={{ color: "#555" }}>{t("wellPlayed", { name: playerName })}</h2>

      <div
        style={{
          backgroundColor: "white",
          padding: "30px",
          borderRadius: "15px",
          boxShadow: "0 4px 10px rgba(0,0,0,0.1)",
          marginTop: "30px",
          textAlign: "center",
        }}
      >
        <p style={{ fontSize: "1.2rem", marginBottom: "20px" }}>
          {t("spottedDifference")}
        </p>

        <button
          onClick={handleRestart}
          style={{
            padding: "15px 30px",
            fontSize: "1.2rem",
            color: "white",
            backgroundColor: "#2e7d32",
            border: "none",
            borderRadius: "8px",
            cursor: "pointer",
          }}
        >
          {t("tryAgain")}
        </button>
      </div>
    </div>
  );
}
