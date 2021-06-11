import reactRefresh from "@vitejs/plugin-react-refresh";
import { defineConfig } from "vite";
import ViteFonts from "vite-plugin-fonts";
import tsconfigPaths from "vite-tsconfig-paths";

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [
        reactRefresh(),
        tsconfigPaths(),
        ViteFonts({
            google: {
                families: ["Source Sans Pro", "Roboto", "Material+Icons"],
            },
        }),
    ],
});
