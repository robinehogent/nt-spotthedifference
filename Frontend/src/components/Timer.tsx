import { useEffect, useState } from "react";

interface TimerProps {
  seconds: number;
  onFinished?: () => void;
}

export default function Timer({ seconds, onFinished }: TimerProps) {
  const [time, setTime] = useState(seconds);

  useEffect(() => {
    if (time <= 0) {
      if (onFinished) onFinished();
      return;
    }

    const id = setInterval(() => {
      setTime((t) => t - 1);
    }, 1000);

    return () => clearInterval(id);
  }, [time, onFinished]);

  return (
    <div
      style={{
        position: "absolute",
        top: "10px",
        right: "10px",
        fontSize: "24px",
        fontWeight: "bold",
      }}
    >
      {time}
    </div>
  );
}
