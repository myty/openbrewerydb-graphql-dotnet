import React from "react";
import { Box, Heading, Flex, Text, Button, FlexProps } from "@chakra-ui/core";

type HeaderProps = {
    title: string;
} & Omit<
    FlexProps,
    "as" | "align" | "justify" | "wrap" | "padding" | "bg" | "color"
>;

const MenuItems = ({ children }: React.PropsWithChildren<{}>) => (
    <Text mt={{ base: 4, md: 0 }} mr={6} display="block">
        {children}
    </Text>
);

const Header = (props: HeaderProps) => {
    const [show, setShow] = React.useState(false);
    const handleToggle = () => setShow(!show);

    return (
        <Flex
            as="nav"
            align="center"
            justify="space-between"
            wrap="wrap"
            padding="1.5rem"
            bg="teal.500"
            color="white"
            {...props}>
            <Flex align="center" mr={5}>
                <Heading as="h1" size="lg" letterSpacing={"-.1rem"}>
                    {props.title}
                </Heading>
            </Flex>

            <Box display={{ sm: "block", md: "none" }} onClick={handleToggle}>
                <svg
                    fill="white"
                    width="12px"
                    viewBox="0 0 20 20"
                    xmlns="http://www.w3.org/2000/svg">
                    <title>Menu</title>
                    <path d="M0 3h20v2H0V3zm0 6h20v2H0V9zm0 6h20v2H0v-2z" />
                </svg>
            </Box>

            <Box
                display={{ sm: show ? "block" : "none", md: "flex" }}
                width={{ sm: "full", md: "auto" }}
                alignItems="center"
                flexGrow={1}>
                <MenuItems>Favorites</MenuItems>
                <MenuItems>Reviews</MenuItems>
            </Box>

            <Box
                display={{ sm: show ? "block" : "none", md: "block" }}
                mt={{ base: 4, md: 0 }}>
                <Button bg="transparent" border="1px">
                    Create account
                </Button>
            </Box>
        </Flex>
    );
};

export default Header;
