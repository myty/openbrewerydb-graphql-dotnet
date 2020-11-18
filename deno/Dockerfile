FROM frolvlad/alpine-glibc:alpine-3.12_glibc-2.32

ENV DENO_VERSION=1.5.2

RUN apk add --virtual .download --no-cache curl \
    && curl -fsSL https://github.com/denoland/deno/releases/download/v${DENO_VERSION}/deno-x86_64-unknown-linux-gnu.zip \
    --output deno.zip \
    && unzip deno.zip \
    && rm deno.zip \
    && chmod 777 deno \
    && mv deno /bin/deno \
    && apk del .download

RUN addgroup -g 1993 -S deno \
    && adduser -u 1993 -S deno -G deno \
    && mkdir /deno-dir/ \
    && chown deno:deno /deno-dir/

ENV DENO_DIR /deno-dir/
ENV DENO_INSTALL_ROOT /usr/local

# The port that your application listens to.
EXPOSE 1993

WORKDIR /app

# Prefer not to run as root.
USER deno

COPY tsconfig.json .

# Cache the dependencies as a layer (the following two steps are re-run only when deps.ts is modified).
# Ideally cache deps.ts will download and compile _all_ external files used in main.ts.
COPY deps.ts .
RUN deno cache deps.ts -c tsconfig.json --unstable

# These steps will be re-run upon each file change in your working directory:
ADD . .
# Compile the main app so that it doesn't need to be compiled each startup/entry.
RUN deno cache main.ts -c tsconfig.json --unstable

CMD ["deno", "run", "--allow-net", "--unstable", "main.ts"]
