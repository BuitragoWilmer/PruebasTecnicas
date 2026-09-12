export interface Link {
  href: string | null;
  rel: string | null;
  method: string | null;
}

export interface Resource<T> {
  data: T;
  links: Link[] | null;
}

export interface ResourceCollection<T> {
  items: T[] | null;
  links: Link[] | null;
}

export interface ProblemDetails {
  type?: string | null;
  title?: string | null;
  status?: number | null;
  detail?: string | null;
  instance?: string | null;
  [key: string]: any;
}