import List from "../ListGroup/List"
import RecommendationBar from "./RecomendationBar";

import './Main.css'

export default function Main() {
    return (
      <>
        <div className="main-container">
          <div className="categories">
            <List />
          </div>
  
          <div className="Widget">
            {/* Ваш віджет тут */}
          </div>

          <div className = "recommend-bar">
            <RecommendationBar/>
          </div>
        </div>
      </>
    );
  }
  