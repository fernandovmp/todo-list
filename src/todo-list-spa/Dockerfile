FROM node AS build-env
WORKDIR /app
ENV PATH ./node_modules/.bin:$PATH
COPY package.json .
RUN npm install

COPY . .

ARG api_url
RUN sh create-env-file.sh ${api_url}

RUN npm run build

FROM nginx
COPY --from=build-env /app/nginx/nginx.conf /etc/nginx/
COPY --from=build-env /app/build /usr/share/nginx/html
EXPOSE 80
