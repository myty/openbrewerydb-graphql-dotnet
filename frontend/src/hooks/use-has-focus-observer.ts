import { RefObject, useCallback, useEffect } from "react";

type UseHasFocusObserverFunction = (
    callback: (hasFocus: boolean) => void,
    elementRef: RefObject<HTMLElement>
) => void;

export const useHasFocusObserver: UseHasFocusObserverFunction = (
    callback,
    elementRef
) => {
    const onChange = useCallback(
        (e: FocusEvent | MouseEvent) => {
            if (elementRef.current == null) {
                callback(false);
                return;
            }

            for (
                let el = e.target as (Node & ParentNode) | null;
                el;
                el = el.parentNode
            ) {
                if (el === elementRef.current) {
                    callback(true);
                    return;
                }
            }

            callback(false);
            return;
        },
        [callback, elementRef]
    );

    useEffect(() => {
        document.addEventListener("focusin", onChange);
        document.addEventListener("click", onChange);

        return () => {
            document.removeEventListener("focusin", onChange);
            document.removeEventListener("click", onChange);
        };
    }, [callback, onChange]);
};
