const webpack = require('webpack')
const ReactRefreshWebpackPlugin = require('@pmmmwh/react-refresh-webpack-plugin')
const { env } = require('process')

require('dotenv').config({ path: './.env.development.local' })

const target = env.ASPNETCORE_HTTPS_PORT
    ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}`
    : env.ASPNETCORE_URLS
        ? env.ASPNETCORE_URLS.split(';')[0]
        : 'https://localhost:44372'

module.exports = {
  mode: 'development',
  devtool: 'cheap-module-source-map',
  devServer: {
    historyApiFallback: true,
    hot: true,
    open: false,
    proxy: {
      '/api': {
        target: 'https://localhost:5101',
        secure: false,
      },
      '/files': {
        target: 'https://localhost:5101',
        secure: false,
      },
    },
    port: 8080,
    https: {
      cert: env.SSL_CRT_FILE,
      key: env.SSL_KEY_FILE,
    },
  },
  plugins: [
    new ReactRefreshWebpackPlugin(),
    new webpack.DefinePlugin({
      'process.env.name': JSON.stringify('mvpdebug'),
    }),
  ],
}
