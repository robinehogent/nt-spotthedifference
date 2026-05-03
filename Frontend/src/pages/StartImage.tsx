import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import Timer from "../components/Timer";
import { t } from "../translations";

const API_URL = import.meta.env.VITE_API_URL || "";

export default function StartImage() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [imageUrl, setImageUrl] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const startGame = async () => {
      try {
        const username = localStorage.getItem("username");
        const language = localStorage.getItem("language") || "en";

        const response = await fetch(`${API_URL}/api/Game/start`, {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({
            username: username,
            difficultyId: Number(id),
            language: language,
          }),
        });

        if (response.status === 404) {
          navigate("/results");
          return;
        }

        if (!response.ok) throw new Error("Start error");

        const data = await response.json();

        localStorage.setItem(
          "currentPlayerRoundId",
          data.playerRoundId.toString()
        );

        localStorage.setItem("currentLevelImage", data.originalImageUrl);
        localStorage.setItem("currentLevelDiffImage", data.differenceImageUrl);

        localStorage.setItem("questionText", data.questionText);

        // answers is now array of objects: [{ answerId, text }]
        localStorage.setItem("answers", JSON.stringify(data.answers));

        // store the ID instead of the text
        localStorage.setItem("correctAnswerId", String(data.correctAnswerId));

        setImageUrl(data.originalImageUrl);
      } catch (error) {
        console.error("Error starting game:", error);
        alert(t("serverError"));
      } finally {
        setLoading(false);
      }
    };

    startGame();
  }, [id, navigate]);

  const handleTimeUp = () => {
    navigate(`/level/${id}/question`);
  };

  if (loading)
    return (
      <div style={{ textAlign: "center", marginTop: 50 }}>{t("loading")}</div>
    );
  if (!imageUrl) return null;

  return (
    <div style={{ textAlign: "center", height: "100vh", padding: 20 }}>
      <h2 style={{ marginBottom: "10px" }}>{t("memorizeImage")}</h2>
      <Timer seconds={5} onFinished={handleTimeUp} />

      <div style={{ marginTop: 20 }}>
        <img
          src={imageUrl}
          alt="Original"
          style={{
            maxWidth: "100%",
            maxHeight: "80vh",
            border: "2px solid #333",
            borderRadius: "8px",
          }}
        />
      </div>
    </div>
  );
}
