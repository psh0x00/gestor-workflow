export interface Estado {
  nome: string;
  descricao?: string;
  corHexadecimal?: string;
  tipo?: number | string;
  funcoes?: string[];
}

export interface Transicao {
  nomeEstadoOrigem?: string;
  nomeEstadoDestino?: string;
  origem?: string;
  destino?: string;
  nome?: string;
  descricao?: string;
}

export interface WorkflowModelo {
  id?: number;
  nome: string;
  descricao?: string;
  estados?: Estado[];
  transicoes?: Transicao[];
  criadoPorId?: number;
  dataCriacao?: string;
  [key: string]: any;
}
