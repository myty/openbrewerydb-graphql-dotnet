import React, { PureComponent } from "react";

interface AppState {
    breweries: any[];
}

const BODY_BG_CLASS = "bg-gray-100";

class App extends PureComponent<unknown, AppState> {
    constructor(props: unknown) {
        super(props);

        this.state = {
            breweries: [],
        };
    }

    componentDidMount() {
        document.body.classList.add(BODY_BG_CLASS);
    }

    componentWillUnmount() {
        document.body.classList.remove(BODY_BG_CLASS);
    }

    render() {
        return (
            <div>
                <div
                    id="header"
                    className="bg-gray-700 text-center font-bold text-white p-3 shadow-lg">
                    <h1>openbrewerydb (React, GraphQL & .NET Core)</h1>
                </div>
                <div id="body" className="p-4"></div>
            </div>
        );
    }
}

export default App;
