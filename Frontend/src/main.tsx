// import React from "react";
import ReactDOM from "react-dom/client";
import { BrowserRouter, Routes, Route } from "react-router-dom";
//import App from "./App";
import StartImage from "./pages/StartImage";
import Question from "./pages/Question";
import Differences from "./pages/Differences";
import Results from "./pages/Results";
import Menu from "./pages/StartMenu";
import AdminPage from "./pages/AdminPage";
import "./Styles/Global.css";


ReactDOM.createRoot(document.getElementById("root")!).render(
  //<React.StrictMode> dublicatie in DB erdoor
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Menu />} />
        <Route path="/level/:id/start" element={<StartImage />} />
        <Route path="/level/:id/question" element={<Question />} />
        <Route path="/level/:id/differences" element={<Differences />} />
        <Route path="/results" element={<Results />} />
        <Route path="/admin" element={<AdminPage />} />
      </Routes>
    </BrowserRouter>
 // </React.StrictMode>
);
