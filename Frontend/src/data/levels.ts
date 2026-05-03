// import EasyBefore from "../assets/Images/easybefore.png";
// import EasyAfter from "../assets/Images/easyafter.png";
// import Easy2Before from "../assets/Images/easy2before.png";
// import Easy2After from "../assets/Images/easy2after.png";
// import MediumBefore from "../assets/Images/mediumbefore.png";
// import MediumAfter from "../assets/Images/mediumafter.png";
// import Medium2Before from "../assets/Images/medium2before.png";
// import Medium2After from "../assets/Images/medium2after.png";
// import HardBefore from "../assets/Images/hardbefore.png";
// import HardAfter from "../assets/Images/hardafter.png";

export interface AnswerResponse {
  answerId: number;
  text: string;
}

export interface LevelData {
  id: number;
  image1: string;
  image2: string;
  question: string;

  answers: AnswerResponse[];
  correctAnswerId: number;

  timeLimitSeconds: number;
}


// export const levels: LevelData[] = [
//   {
//     id: 1,
//     image1: EasyBefore,
//     image2: EasyAfter,

//     question: "What color shirt was the boy wearing?",
//     answers: ["Orange", "Purple", "Green"],
//     correctAnswer: "Orange",

//     differenceOptions: [
//       "A tree disappeared",
//       "The ball changed colour",
//       "A second cloud appeared"
//     ],
//     correctDifference: "The ball changed colour"
//   },

//   {
//     id: 2,
//     image1: Easy2Before,
//     image2: Easy2After,

//     question: "How many balloons were in the picture?",
//     answers: ["2", "3", "4"],
//     correctAnswer: "3",

//     differenceOptions: [
//       "An extra balloon appeared",
//       "The dog changed colour",
//       "The cat is not wearing a hat anymore"
//     ],
//     correctDifference: "The cat is not wearing a hat anymore"
//   },

//   {
//     id: 3,
//     image1: MediumBefore,
//     image2: MediumAfter,

//     question: "What was the lady holding in her left hand?",
//     answers: ["A knife", "A Fork", "A Spoon"],
//     correctAnswer: "A knife",

//     differenceOptions: [
//       "The lady is wearing an apron now",
//       "The time on the clock changed",
//       "She is now holding a knife instead of a fork"
//     ],
//     correctDifference: "The time on the clock changed"
//   },

//   {
//     id: 4,
//     image1: Medium2Before,
//     image2: Medium2After,

//     question: "What was one of the shapes on the neon signs?",
//     answers: ["A Triangle", "A Star", "A Heart"],
//     correctAnswer: "A Star",

//     differenceOptions: [
//       "The neon sign changed colour",
//       "A cat appeared on the balcony",
//       "Extra clouds appeared"
//     ],
//     correctDifference: "Extra clouds appeared"
//   },

//   {
//     id: 5,
//     image1: HardBefore,
//     image2: HardAfter,

//     question: "How many people were in the museum?",
//     answers: ["4", "6", "8"],
//     correctAnswer: "6",

//     differenceOptions: [
//       "An extra person appeared",
//       "The vase changed inside the display case",
//       "The Trex skeleton lost a tooth"
//     ],
//     correctDifference: "The vase changed inside the display case"
//   }
//];
