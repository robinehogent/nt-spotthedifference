import { useState, useRef, type ChangeEvent, type MouseEvent } from "react";
import { useNavigate } from "react-router-dom";
import { t } from "../translations";

const API_URL = import.meta.env.VITE_API_URL || "";

type DifferenceMark = { id: number; x: number; y: number };

const AdminPage = () => {
  const navigate = useNavigate();
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [password, setPassword] = useState("");

  const [roundName, setRoundName] = useState("");
  const [difficulty, setDifficulty] = useState("EASY");
  const [file1, setFile1] = useState<File | null>(null);
  const [file2, setFile2] = useState<File | null>(null);
  const [preview2, setPreview2] = useState<string | null>(null);

  const [differences, setDifferences] = useState<DifferenceMark[]>([]);
  const imgRef = useRef<HTMLImageElement>(null);

  const [questionText, setQuestionText] = useState("");
  const [answer1, setAnswer1] = useState("");
  const [answer2, setAnswer2] = useState("");
  const [answer3, setAnswer3] = useState("");
  const [correctAnswerIndex, setCorrectAnswerIndex] = useState(0);

  const [isUploading, setIsUploading] = useState(false);

  const handleLogin = () => {
    if (password === "1111") setIsLoggedIn(true);
    else alert(t("wrongPass"));
  };

  const handleFile2Change = (e: ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files[0]) {
      const file = e.target.files[0];
      setFile2(file);
      setPreview2(URL.createObjectURL(file));
      setDifferences([]);
    }
  };

  const handleImageClick = (e: MouseEvent<HTMLImageElement>) => {
    if (!imgRef.current) return;
    const rect = imgRef.current.getBoundingClientRect();
    const x = e.clientX - rect.left;
    const y = e.clientY - rect.top;
    setDifferences([
      ...differences,
      { id: Date.now(), x: Math.round(x), y: Math.round(y) },
    ]);
  };

  const removeDifference = (id: number) => {
    setDifferences(differences.filter((d) => d.id !== id));
  };

  const uploadImage = async (file: File) => {
    const formData = new FormData();
    formData.append("file", file);
    const response = await fetch(`${API_URL}/api/admin/upload`, {
      method: "POST",
      body: formData,
    });
    if (!response.ok) throw new Error("Upload failed");
    return (await response.json()).path;
  };

  const handleSaveRound = async () => {
    if (
      !roundName ||
      !file1 ||
      !file2 ||
      !questionText ||
      !answer1 ||
      !answer2 ||
      !answer3
    ) {
      alert(t("fillAllFields"));
      return;
    }
    if (differences.length === 0) {
      alert(t("markAtLeastOne"));
      return;
    }

    setIsUploading(true);
    try {
      const img = imgRef.current;
      let finalDifferences = differences;

      if (img) {
        const scaleX = img.naturalWidth / img.width;
        const scaleY = img.naturalHeight / img.height;

        finalDifferences = differences.map((d) => ({
          ...d,
          x: Math.round(d.x * scaleX),
          y: Math.round(d.y * scaleY),
        }));
      }
      const path1 = await uploadImage(file1);
      const path2 = await uploadImage(file2);

      const answers = [
        { text: answer1, isCorrect: correctAnswerIndex === 0 },
        { text: answer2, isCorrect: correctAnswerIndex === 1 },
        { text: answer3, isCorrect: correctAnswerIndex === 2 },
      ];

      const payload = {
        name: roundName,
        difficulty: difficulty,
        // description: description,
        originalImage: path1,
        differenceImage: path2,

        differences: finalDifferences.map((d) => ({ x: d.x, y: d.y })),

        questionText: questionText,
        answers: answers,
      };

      const dbResponse = await fetch(`${API_URL}/api/admin/create-round`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload),
      });

      if (!dbResponse.ok) throw new Error("Failed to save round");

      alert(t("roundSaved"));
      navigate("/");
    } catch (error) {
      console.error(error);
      alert(t("somethingWentWrong"));
    } finally {
      setIsUploading(false);
    }
  };

  if (!isLoggedIn)
    return (
      <div style={styles.container}>
        <h2>{t("adminLogin")}</h2>
        <input
          type="password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          style={styles.input}
          placeholder={t("passwordPlaceholder")}
        />
        <button onClick={handleLogin} style={styles.button}>
          {t("login")}
        </button>
      </div>
    );

  return (
    <div style={styles.container}>
      <h2>{t("addNewRound")}</h2>

      <div style={styles.section}>
        <h3>1. {t("generalInfo")}</h3>
        <label>{t("roundName")}:</label>
        <input
          style={styles.input}
          value={roundName}
          onChange={(e) => setRoundName(e.target.value)}
          placeholder={t("roundNamePlaceholder")}
        />

        <label>{t("selectDifficulty")}</label>
        <select
          style={styles.input}
          value={difficulty}
          onChange={(e) => setDifficulty(e.target.value)}
        >
          <option value="EASY">{t("easy")}</option>
          <option value="MEDIUM">{t("medium")}</option>
          <option value="HARD">{t("hard")}</option>
        </select>
      </div>

      <div style={styles.section}>
        <h3>2. {t("imagesAndDifferences")}</h3>
        <div style={{ marginBottom: 10 }}>
          <label>{t("original")}:</label>
          <input
            type="file"
            onChange={(e) => setFile1(e.target.files?.[0] || null)}
          />
        </div>
        <div style={{ marginBottom: 10 }}>
          <label>{t("withDifferences")}:</label>
          <input type="file" onChange={handleFile2Change} />
        </div>

        {preview2 && (
          <div
            style={{
              position: "relative",
              display: "inline-block",
              border: "2px solid #333",
            }}
          >
            <img
              ref={imgRef}
              src={preview2}
              onClick={handleImageClick}
              style={{
                maxWidth: "100%",
                cursor: "crosshair",
                display: "block",
              }}
            />
            {differences.map((d) => (
              <div
                key={d.id}
                onClick={(e) => {
                  e.stopPropagation();
                  removeDifference(d.id);
                }}
                style={{
                  position: "absolute",
                  top: d.y - 20,
                  left: d.x - 20,
                  width: 40,
                  height: 40,
                  border: "3px solid red",
                  borderRadius: "50%",
                  background: "rgba(255,0,0,0.2)",
                  cursor: "pointer",
                }}
              />
            ))}
          </div>
        )}
        <p style={{ fontSize: "0.9em", color: "#666" }}>
          {t("markDifferencesInstruction", { count: differences.length })}
        </p>
      </div>

      <div style={styles.section}>
        <h3>3. {t("questionAndAnswers")}</h3>
        <label>{t("question")}:</label>
        <input
          style={styles.input}
          value={questionText}
          onChange={(e) => setQuestionText(e.target.value)}
          placeholder={t("questionPlaceholder")}
        />

        <div style={{ marginTop: 10 }}>
          <label>
            {t("answers")} ({t("selectCorrect")})
          </label>

          {[answer1, answer2, answer3].map((_, idx) => (
            <div
              key={idx}
              style={{
                display: "flex",
                alignItems: "center",
                gap: 10,
                marginTop: 5,
              }}
            >
              <input
                type="radio"
                name="correctAnswer"
                checked={correctAnswerIndex === idx}
                onChange={() => setCorrectAnswerIndex(idx)}
              />
              <input
                style={{ ...styles.input, flex: 1 }}
                value={idx === 0 ? answer1 : idx === 1 ? answer2 : answer3}
                onChange={(e) => {
                  if (idx === 0) setAnswer1(e.target.value);
                  if (idx === 1) setAnswer2(e.target.value);
                  if (idx === 2) setAnswer3(e.target.value);
                }}
                placeholder={t("answerPlaceholder", { number: idx + 1 })}
              />
            </div>
          ))}
        </div>
      </div>

      <button
        onClick={handleSaveRound}
        disabled={isUploading}
        style={styles.saveButton}
      >
        {isUploading ? t("savingRound") : t("saveRound")}
      </button>

      <button
        onClick={() => navigate("/")}
        style={{ ...styles.button, background: "#ccc", marginTop: 10 }}
      >
        {t("back")}
      </button>
    </div>
  );
};

const styles = {
  container: {
    maxWidth: "700px",
    margin: "20px auto",
    padding: 20,
    fontFamily: "Arial",
    background: "#fff",
    borderRadius: 8,
    boxShadow: "0 2px 10px rgba(0,0,0,0.1)",
  },
  section: {
    marginBottom: 20,
    padding: 15,
    background: "#f9f9f9",
    borderRadius: 8,
    border: "1px solid #eee",
  },
  input: {
    padding: 8,
    borderRadius: 4,
    border: "1px solid #ccc",
    width: "100%",
    boxSizing: "border-box" as const,
    marginTop: 5,
    marginBottom: 10,
  },
  button: {
    padding: "10px 20px",
    background: "#007BFF",
    color: "white",
    border: "none",
    borderRadius: 4,
    cursor: "pointer",
  },
  saveButton: {
    width: "100%",
    padding: 15,
    background: "#28a745",
    color: "white",
    border: "none",
    borderRadius: 4,
    cursor: "pointer",
    fontSize: 16,
    fontWeight: "bold",
  },
};

export default AdminPage;
