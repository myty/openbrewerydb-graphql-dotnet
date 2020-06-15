# pull official base image
FROM node:12.18.0-alpine as node

# set working directory
WORKDIR /app

# add `/app/node_modules/.bin` to $PATH
ENV PATH /app/node_modules/.bin:$PATH

# install app dependencies
COPY package.json ./
COPY yarn.lock ./
RUN yarn install
RUN yarn global add react-scripts@3.4.1

# add app
COPY . ./

# start app
CMD ["yarn", "start"]
