import { useState } from "react";
import { type MouseEvent } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { t } from "../translations";

const API_URL = import.meta.env.VITE_API_URL || "";

export default function Differences() {
  const navigate = useNavigate();
  const { id } = useParams();

  const originalImageSrc = localStorage.getItem("currentLevelImage");
  const imageSrc = localStorage.getItem("currentLevelDiffImage");

  const [message, setMessage] = useState<string>(t("clickDifference"));
  const [markers, setMarkers] = useState<
    { xPercent: number; yPercent: number; color: string }[]
  >([]);
  const [locked, setLocked] = useState(false);

  const handleImageClick = async (e: MouseEvent<HTMLImageElement>) => {
    if (locked) return;

    const img = e.currentTarget;
    const rect = img.getBoundingClientRect();

    const clickX = e.clientX - rect.left;
    const clickY = e.clientY - rect.top;
    const xPercent = clickX / rect.width;
    const yPercent = clickY / rect.height;

    const scaleX = img.naturalWidth / img.width;
    const scaleY = img.naturalHeight / img.height;
    const realX = Math.round(clickX * scaleX);
    const realY = Math.round(clickY * scaleY);

    const roundId = localStorage.getItem("currentPlayerRoundId");

    try {
      const response = await fetch(`${API_URL}/api/Game/guess`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          playerRoundId: Number(roundId),
          x: realX,
          y: realY,
        }),
      });

      const data = await response.json();

      if (data.isCorrect) {
        setMessage(t("correct"));
        setMarkers((prev) => [
          ...prev,
          { xPercent, yPercent, color: "green" },
        ]);
        setLocked(true);

        setTimeout(() => {
          navigate(`/level/${id}/start`);
        }, 1500);
      } else {
        setMessage(t("miss"));
        setMarkers((prev) => [...prev, { xPercent, yPercent, color: "red" }]);
      }
    } catch (error) {
      console.error(error);
      alert(t("networkError"));
    }
  };

  if (!imageSrc || !originalImageSrc)
    return (
      <div style={{ textAlign: "center", marginTop: 50 }}>
        {t("noImageError")}
      </div>
    );

  return (
    <div style={{ textAlign: "center", height: "100vh", padding: 20 }}>
      <h2 style={{ marginBottom: 20 }}>{message}</h2>

      <div
        style={{
          display: "flex",
          justifyContent: "center",
          gap: 20,
          alignItems: "flex-start",
          flexWrap: "wrap",
        }}
      >
        {[
          { src: originalImageSrc, alt: "Original image" },
          { src: imageSrc, alt: "New image" },
        ].map((image) => (
          <div
            key={image.alt}
            style={{
              position: "relative",
              display: "inline-block",
            }}
          >
            <img
              src={image.src}
              alt={image.alt}
              onClick={handleImageClick}
              style={{
                maxWidth: "min(44vw, 700px)",
                width: "100%",
                maxHeight: "80vh",
                cursor: "crosshair",
                border: "2px solid black",
                display: "block",
              }}
            />

            {markers.map((m, index) => (
              <div
                key={`${image.alt}-${index}`}
                style={{
                  position: "absolute",
                  left: `calc(${m.xPercent * 100}% - 20px)`,
                  top: `calc(${m.yPercent * 100}% - 20px)`,
                  width: "40px",
                  height: "40px",
                  border: `3px solid ${m.color}`,
                  borderRadius: "50%",
                  pointerEvents: "none",
                }}
              />
            ))}
          </div>
        ))}
      </div>
    </div>
  );
}
