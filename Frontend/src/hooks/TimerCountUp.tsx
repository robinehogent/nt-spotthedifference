import { useEffect, useState } from "react";

export function TimerCountUp(start: boolean = true) {
  const [seconds, setSeconds] = useState(0);

  useEffect(() => {
    if (!start) return;

    const id = setInterval(() => {
      setSeconds((s) => s + 1);
    }, 1000);

    return () => clearInterval(id);
  }, [start]);

  return seconds;
}
