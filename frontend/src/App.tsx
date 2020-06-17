import React, { PureComponent } from "react";

interface AppState {
    breweries: any[];
}

const BODY_BG_CLASS = "bg-gray-100";

class App extends PureComponent<unknown, AppState> {
    constructor(props: unknown) {
        super(props);

        this.state = {
            breweries: [
                { id: 1, name: "test1" },
                { id: 2, name: "test2" },
                { id: 3, name: "test3" },
                { id: 4, name: "test4" },
            ],
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
                    <h1>OpenBreweryDB (React, GraphQL & .NET Core)</h1>
                </div>
                <div id="body" className="p-4">
                    {this.state.breweries.map((b) => (
                        <div key={b.id}>{b.name}</div>
                    ))}
                </div>
            </div>
        );
    }
}

export default App;
