import { useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import Page from "../components/Page";
import { TimerCountUp } from "../hooks/TimerCountUp";
import { t } from "../translations";

interface AnswerResponse {
  answerId: number;
  text: string;
}

export default function Question() {
  const navigate = useNavigate();
  const { id } = useParams();

  const questionText = localStorage.getItem("questionText") ?? "";

  const answers: AnswerResponse[] = JSON.parse(
    localStorage.getItem("answers") ?? "[]"
  );

  const correctAnswerId = Number(localStorage.getItem("correctAnswerId"));

  const timeTaken = TimerCountUp();

  const [selectedId, setSelectedId] = useState<number | null>(null);
  const [submitted, setSubmitted] = useState(false);

  const handleSubmit = () => {
    if (selectedId === null) return;

    setSubmitted(true);

    localStorage.setItem(
      "questionCorrect",
      String(selectedId === correctAnswerId)
    );
    localStorage.setItem("questionTime", String(timeTaken));

    setTimeout(() => {
      navigate(`/level/${id}/differences`);
    }, 1500);
  };

  return (
    <Page>
      <div
        style={{ maxWidth: "600px", margin: "80px auto", textAlign: "center" }}
      >
        <h2 style={{ marginBottom: "30px" }}>{questionText}</h2>

        <div style={{ display: "flex", flexDirection: "column", gap: "12px" }}>
          {answers.map((answer) => {
            const isCorrect = submitted && answer.answerId === correctAnswerId;
            const isWrongSelection =
              submitted && selectedId === answer.answerId && !isCorrect;

            return (
              <label
                key={answer.answerId}
                style={{
                  border: "1px solid #ccc",
                  padding: "10px 14px",
                  borderRadius: "8px",
                  cursor: "pointer",
                  background: submitted
                    ? isCorrect
                      ? "#4caf50"
                      : isWrongSelection
                      ? "#ff6b6b"
                      : "white"
                    : selectedId === answer.answerId
                    ? "#e8f0fe"
                    : "white",
                  color: isCorrect || isWrongSelection ? "white" : "inherit",
                }}
              >
                <input
                  type="radio"
                  name="question"
                  value={answer.answerId}
                  checked={selectedId === answer.answerId}
                  onChange={() => !submitted && setSelectedId(answer.answerId)}
                  disabled={submitted}
                  style={{ marginRight: "8px" }}
                />
                {answer.text}
              </label>
            );
          })}
        </div>

        <button
          onClick={handleSubmit}
          disabled={selectedId === null || submitted}
          style={{
            marginTop: "30px",
            padding: "10px 22px",
            fontSize: "18px",
            borderRadius: "8px",
            cursor:
              selectedId !== null && !submitted ? "pointer" : "not-allowed",
            backgroundColor:
              selectedId !== null && !submitted ? "#4caf50" : "#ccc",
            color: "white",
            border: "none",
          }}
        >
          {t("submit")}
        </button>
      </div>
    </Page>
  );
}
