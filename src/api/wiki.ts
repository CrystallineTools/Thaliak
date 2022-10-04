class WikiClient {
  constructor(private baseUrl: string) {}

  async get(path: string) {
    const response = await fetch(`${this.baseUrl}/${path}`);
    return response.json();
  }
}

export const Wiki = new WikiClient(process.env.REACT_APP_WIKI_API ?? 'https://thinwiki.omphalos.dev');
