import React from "react";
import { Spinner, Text, Box } from "@chakra-ui/core";

interface LoadingProps {
    loadingText?: string;
}

export const Loading = ({ loadingText }: LoadingProps) => {
    return (
        <Box w="100%" textAlign="center">
            <Spinner size="xl" />
            <Text>{loadingText ?? "Loading..."}</Text>
        </Box>
    );
};
