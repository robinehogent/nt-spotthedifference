export interface GameState {
  questionCorrect: boolean;
  questionTime: number;

  differenceCorrect: boolean;
  differenceTime: number;
}

export const gameState: GameState = {
  questionCorrect: false,
  questionTime: 0,

  differenceCorrect: false,
  differenceTime: 0
};
