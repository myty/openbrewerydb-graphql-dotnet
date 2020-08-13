import React, { useEffect, useCallback, useMemo } from "react";

interface ModalProps {
    actionButtonText?: string;
    onAction?: () => void;
    onClose?: () => void;
    showModal?: boolean;
    title: string;
}

export const Modal = ({
    actionButtonText = "Submit",
    children,
    onAction = () => {},
    onClose = () => {},
    showModal = false,
    title,
}: React.PropsWithChildren<ModalProps>) => {
    const onKeyDown = useCallback(
        (evt: KeyboardEvent) => {
            evt = evt ?? window.event;
            const isEscape = evt.key === "Escape" || evt.key === "Esc";

            if (isEscape && showModal) {
                onClose();
            }
        },
        [onClose, showModal]
    );

    useEffect(() => {
        if (!showModal) {
            document.querySelector("body")?.classList.remove("modal-active");
        } else {
            document.querySelector("body")?.classList.add("modal-active");
        }
    }, [showModal]);

    useEffect(() => {
        document.addEventListener("keydown", onKeyDown);

        // Cleanup the event handler on unmount
        return () => document.removeEventListener("keydown", onKeyDown);
    }, [onKeyDown]);

    const showClassName = useMemo(
        () => (!showModal ? "opacity-0 pointer-events-none" : ""),
        [showModal]
    );

    const focusableTabIndex = useMemo(() => (!showModal ? -1 : 0), [showModal]);

    return (
        <div
            style={{ zIndex: 9999 }}
            className={`modal ${showClassName} fixed w-full h-full top-0 left-0 flex items-center justify-center`}>
            <div
                onClick={onClose}
                className="absolute w-full h-full bg-gray-900 opacity-50 modal-overlay"
                tabIndex={focusableTabIndex}></div>

            <div className="z-50 w-11/12 mx-auto overflow-y-auto bg-white rounded shadow-lg modal-container md:max-w-md">
                <button
                    onClick={onClose}
                    className="absolute top-0 right-0 z-50 flex flex-col items-center mt-4 mr-4 text-sm text-white cursor-pointer modal-close"
                    tabIndex={focusableTabIndex}>
                    <svg
                        className="text-white fill-current"
                        xmlns="http://www.w3.org/2000/svg"
                        width="18"
                        height="18"
                        viewBox="0 0 18 18">
                        <path d="M14.53 4.53l-1.06-1.06L9 7.94 4.53 3.47 3.47 4.53 7.94 9l-4.47 4.47 1.06 1.06L9 10.06l4.47 4.47 1.06-1.06L10.06 9z"></path>
                    </svg>
                    <span className="text-sm">(Esc)</span>
                </button>

                <div className="px-6 py-4 text-left modal-content">
                    <div className="flex items-center justify-between pb-3">
                        <p className="text-2xl font-bold">{title}</p>
                        <button
                            className="z-50 cursor-pointer modal-close"
                            tabIndex={focusableTabIndex}
                            onClick={onClose}>
                            <svg
                                className="text-black fill-current"
                                xmlns="http://www.w3.org/2000/svg"
                                width="18"
                                height="18"
                                viewBox="0 0 18 18">
                                <path d="M14.53 4.53l-1.06-1.06L9 7.94 4.53 3.47 3.47 4.53 7.94 9l-4.47 4.47 1.06 1.06L9 10.06l4.47 4.47 1.06-1.06L10.06 9z"></path>
                            </svg>
                        </button>
                    </div>
                    {children}
                    <div className="flex justify-end pt-2">
                        <button
                            tabIndex={focusableTabIndex}
                            onClick={onAction}
                            className="p-3 px-4 mr-2 text-yellow-700 bg-transparent rounded-lg hover:bg-gray-100 hover:text-yellow-600">
                            {actionButtonText}
                        </button>
                        <button
                            tabIndex={focusableTabIndex}
                            onClick={onClose}
                            className="p-3 px-4 text-white bg-yellow-700 rounded-lg modal-close hover:bg-yellow-600">
                            Close
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
};
