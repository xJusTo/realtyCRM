export enum PropertyType {
    Apartment = 0,
    House = 1,
    Commercial = 2
}

export enum PropertyStatus {
    Available = 0,
    Booked = 1,
    Sold = 2
}

export interface Property {
    id: number;
    address: string;
    description: string;
    price: number;
    area: number;
    type: PropertyType;
    status: PropertyStatus;
    realtorId: number;
}
