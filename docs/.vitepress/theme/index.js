import DefaultTheme from 'vitepress/theme'
import Contributors from '../components/Contributors.vue'
import './custom.css'

export default {
  extends: DefaultTheme,
  enhanceApp({ app }) {
    // Register global components
    app.component('Contributors', Contributors)
  }
}
